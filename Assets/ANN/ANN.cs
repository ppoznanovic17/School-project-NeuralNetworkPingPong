using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN{

	public int numInputs;            // broj ulaznih vrednosti na pocetku
	public int numOutputs;          // koliko ima izlaznih vrednosti
	public int numHidden;           // slojevi izmedju ulaza i izlaza
	public int numNPerHidden;       //koliko ima neurona ima u sloju
	public double alpha;            // koliko ce mreza brzo da uci
                                    //determinise koliko ce neki trening set uticati na mrezu
	List<Layer> layers = new List<Layer>();

	public ANN(int nI, int nO, int nH, int nPH, double a)
	{
		numInputs = nI;
		numOutputs = nO;
		numHidden = nH;
		numNPerHidden = nPH;
		alpha = a;

		if(numHidden > 0)
		{
            //pravim ulazni layer
			layers.Add(new Layer(numNPerHidden, numInputs));

			for(int i = 0; i < numHidden-1; i++)
			{
                // pravim layere mreze
				layers.Add(new Layer(numNPerHidden, numNPerHidden));
			}
            // pravim layer za izlazne vrednosti
			layers.Add(new Layer(numOutputs, numNPerHidden));
		}
		else//ako nema layere izmedju izlaza i pocetka
		{   
            // pravim samo layer za izlazne vrednosti
			layers.Add(new Layer(numOutputs, numInputs));
		}
	}

    // glavna metoda koja poziva ostale metode unutar klase
	public List<double> Train(List<double> inputValues, List<double> desiredOutput)
	{
		List<double> outputValues = new List<double>();
		outputValues = CalcOutput(inputValues, desiredOutput);
		UpdateWeights(outputValues, desiredOutput);
		return outputValues;
	}

	public List<double> CalcOutput(List<double> ulazneVrednosti, List<double> zeljeniIzlaz)
	{
		List<double> izlazi = new List<double>();
		List<double> izlazneVrednosti = new List<double>();
		int currentInput = 0;              // pomoc pri menjanju tezina pri treningu

        // ako ima vise ili manje ulaznih vrednosti nego sto treba
		if(ulazneVrednosti.Count != numInputs)
		{
			Debug.Log("ERROR: Number of Inputs must be " + numInputs);
			return izlazneVrednosti;
		}

		izlazi = new List<double>(ulazneVrednosti);

		for(int i = 0; i < numHidden + 1; i++)
		{
				if(i > 0)
				{   
                    // izlaz iz jednog layera postaje ulaz drugog
					izlazi = new List<double>(izlazneVrednosti);
				}
                // praznim izlaz kako bih ga updatovao
				izlazneVrednosti.Clear();
                // prolazim kroz neurone layera
				for(int j = 0; j < layers[i].numNeurons; j++)
				{
					double N = 0;
                    //brisem prethodni ulaz
                    layers[i].neurons[j].inputs.Clear();
                        

					for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
					{   
                        //dodajem nove inpute svakom neuronu
					    layers[i].neurons[j].inputs.Add(izlazi[currentInput]);
                       
						N += layers[i].neurons[j].weights[k] * izlazi[currentInput];
						currentInput++;
					}


					N += layers[i].neurons[j].bias;  

					if(i == numHidden)
						layers[i].neurons[j].output = AktivacionaFunkcija(N);
					else
						layers[i].neurons[j].output = AktivacionaFunkcija(N);
					
                    //dodajemo u autpute , kako bih narednom layeru dodali u input
					izlazneVrednosti.Add(layers[i].neurons[j].output);
					currentInput = 0;
				}
		}
		return izlazneVrednosti;
	}

	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double greska;
		for(int i = numHidden; i >= 0; i--)
		{
			for(int j = 0; j < layers[i].numNeurons; j++)
			{
                // output layer
				if(i == numHidden)
				{
					greska = desiredOutput[j] - outputs[j];
                    
                    // utice na ukupan error
					layers[i].neurons[j].errorGradient = outputs[j] * (1-outputs[j]) * greska;
				}
				else
				{
                    //delta rule
					layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1-layers[i].neurons[j].output);
					double gradijent = 0;
					for(int p = 0; p < layers[i+1].numNeurons; p++)
					{
                        // racunam eror u narednom layeru jer se update vrsi od nazad
                       
						gradijent += layers[i+1].neurons[p].errorGradient * layers[i+1].neurons[p].weights[j];
					}
                    //povecavamo gradient neurona na kome se nalazimo
					layers[i].neurons[j].errorGradient += gradijent;
				}	
				for(int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
                    // output layer
					if(i == numHidden)
					{
						greska = desiredOutput[j] - outputs[j];
                        // racunamo tezinu tako sto mnozim learning rate sa inputom i greskom
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * greska;
					}
					else
					{
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
                // updejtujem bias neurona
				layers[i].neurons[j].bias += (alpha *-1 *layers[i].neurons[j].errorGradient);// bolje kad pomnozim puta 1
			}

		}

	}

	double AktivacionaFunkcija(double value)
	{
		return TanH(value);
	}

	double AktivacionaFunkcijaAutputa(double value)
	{
		return TanH(value);
	}

	double TanH(double value)
	{
        //vraca vrednost izmedju -1,1
		double k = (double) System.Math.Exp(-2*value);
    	return 2 / (1.0f + k) - 1;
	}

    double TanH2(double value)
    {
        return (2 * (Sigmoid(2 * value)) - 1);
    }

	double Sigmoid(double value) 
	{
        double k = (double)System.Math.Exp(value);
    	return k / (1.0f + k);
	}/*
    double Step(double value) // ili ti binary step
    {
        if (value < 0) return 0;
        else return 1;
    }*/
}

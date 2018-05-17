using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Genographer {

    public List<Genome> genomes;
    public int generationTime; //seconds

    public Genographer(int numGenomes, int generationTime)
    {
        this.generationTime = generationTime;
        genomes = new List<Genome>();
        for(int i = 0; i < numGenomes; i++)
        {
            genomes.Add(CreateGenome());
        }
    }

    public Genome CreateGenome()
    {
        int obstacleJump = Random.Range(0, 15);
        int platformJump = Random.Range(0, 15);
        return new Genome(obstacleJump, platformJump);
    }

    public void CreateNextGeneration()
    {
        //Using biased roulette wheel
        List<List<Genome>> pairs = new List<List<Genome>>();
        List<Genome> bestPair = new List<Genome>();
        //First get the total score
        float totalScore = 0;
        Genome bestGenome = genomes[0];
        Genome secondBestGenome = genomes[1];
        foreach(Genome genome in genomes)
        {
            totalScore += genome.score;

            if(genome.score > secondBestGenome.score && genome.score > bestGenome.score)
            {
                bestGenome = genome;
            }
            else if(genome.score > secondBestGenome.score && genome.score < bestGenome.score)
            {
                secondBestGenome = genome;
            }
        }
        bestPair.Add(bestGenome);
        bestPair.Add(secondBestGenome);
        //then work out the proportionality of the scores of each of the genomes
        float totalProportion = 0;
        foreach(Genome genome in genomes)
        {
            genome.upperProportion = totalProportion + ((genome.score / totalScore) * 100);
            genome.lowerProportion = totalProportion;
            totalProportion += genome.upperProportion;
        }
        //now we can create our mating pairs
        while (pairs.Count < (genomes.Count / 2) - 1)
        {
            List<Genome> pair = new List<Genome>();
            while (pair.Count < 2) 
            {
                int roll = Random.Range(0, 100);
                foreach(Genome genome in genomes)
                {
                    if(roll > genome.lowerProportion && roll < genome.upperProportion)
                    {
                        pair.Add(genome.Clone());
                    }
                }
            }
            pairs.Add(pair);
        }
        //now mate the pairs to create a new genome
        List<Genome> newGenomes = new List<Genome>();
        foreach(List<Genome> pair in pairs)
        {
            List<Genome> childPair = Crossover(pair);
            foreach (Genome genome in childPair) 
            {
                newGenomes.Add(genome);
            }
        }
        //Add out elites from the previous generation
        foreach(Genome genome in bestPair)
        {
            newGenomes.Add(genome);
        }
        //We have our next generation!
        genomes = newGenomes;
        string strGenes = "";
        foreach(Genome genome in genomes)
        {
            strGenes += ":" + genome.obstacleToInt() + "," + genome.platformToInt() + ":";
        }
        Debug.Log("BEST PAIR: " + bestPair[0].obstacleToInt() + "," + bestPair[0].platformToInt() + ":" + bestPair[1].obstacleToInt() + "," + bestPair[1].platformToInt());
        Debug.Log("NEW GENE POOL: " + strGenes);
    }

    List<Genome> Crossover(List<Genome> pair)
    {
        List<Genome> children = new List<Genome>();
        //Get the gene sequence
        string[] genesA = pair[0].genome;
        string[] genesB = pair[1].genome;
        //find out where one gene ends and the other begins for later
        int breakGenesA = genesA[0].Length;
        int breakGenesB = genesB[0].Length;
        //Merge the genes for crossover
        List<char> mergedGenesA = genesA[0].ToCharArray().ToList();
            mergedGenesA.AddRange(genesA[1].ToCharArray().ToList());
        List<char> mergedGenesB = genesB[0].ToCharArray().ToList();
            mergedGenesB.AddRange(genesB[1].ToCharArray().ToList());
        //Get random position for crossover
        bool gettingRange = true;
        int crossoverPoint = 0;
        while (gettingRange)
        {
            crossoverPoint = Random.Range(1, mergedGenesA.Count - 1);
            //Make sure our crossoverpoint is in bounds for both sequences
            if(crossoverPoint < mergedGenesB.Count - 1)
            {
                gettingRange = false;
            }
        }
        //Now that we have our position we can perform the crossover;
        List<char> childACharGenome = new List<char>();
        List<char> childBCharGenome = new List<char>();
        for (int i = 0; i < mergedGenesA.Count; i++)
        {
            if(i <= crossoverPoint)
            {
                childACharGenome.Add(mergedGenesB[i]);
            }
            else
            {
                childACharGenome.Add(mergedGenesA[i]);
            }
        }
        for(int i = 0; i < mergedGenesB.Count; i++)
        {
            if(i <= crossoverPoint)
            {
                childBCharGenome.Add(mergedGenesA[i]);
            }
            else
            {
                childBCharGenome.Add(mergedGenesB[i]);
            }
        }
        //We've performed our crossover! See if there will be any mutations in either of the children.
        childACharGenome = Mutate(childACharGenome);
        childBCharGenome = Mutate(childBCharGenome);
        //Now we have to rebuild the children into genome objects, with seperated genes
        List<char> childAGeneA = childACharGenome.Take(breakGenesA).ToList();
        List<char> childAGeneB = childACharGenome.Skip(breakGenesA).ToList();
        List<char> childBGeneA = childBCharGenome.Take(breakGenesB).ToList();
        List<char> childBGeneB = childBCharGenome.Skip(breakGenesB).ToList();

        List<string> childAGenome = new List<string>();
            childAGenome.Add(new string(childAGeneA.ToArray()));
            childAGenome.Add(new string(childAGeneB.ToArray()));
        List<string> childBGenome = new List<string>();
            childBGenome.Add(new string(childBGeneA.ToArray()));
            childBGenome.Add(new string(childBGeneB.ToArray()));
        Genome childA = new Genome(childAGenome.ToArray());
        Genome childB = new Genome(childBGenome.ToArray());
        List<Genome> childPair = new List<Genome>();
            childPair.Add(childA);
            childPair.Add(childB);
        return childPair;
    }

    List<char> Mutate(List<char> genome)
    {
        List<char> mutations = genome;
        for(int i = 0; i < mutations.Count; i++)
        {
            //5% chance of mutation
            int roll = Random.Range(0, 100);
            if(roll < 5)
            {
                if (mutations[i].Equals('1'))
                {
                    mutations[i] = '0';
                }
                else if (mutations[i].Equals('0'))
                {
                    mutations[i] = '1';
                }
            }
        }
        return mutations;
    }
}

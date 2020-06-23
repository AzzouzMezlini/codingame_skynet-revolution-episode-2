using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement
 **/
class Player
{
    static Dictionary<int, Node> nodes;
    static void Main(string[] args)
    {
        // On récupere toutes les entrées
        // Et on initialise l'état initial avant le "game loop"
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        // N représente le nombre de noeuds (y compris les passerelles) du sous-réseau.
        int N = int.Parse(inputs[0]);
        //nodes est le dictionaire des noeuds
        // l'indice correspond au numéro du noeud
        // la valeur à la liste des neouds auquel il est relié
        nodes = Enumerable.Range(0, N).ToDictionary(i => i, i=> new Node()); 
        // L représente le nombre de liens du sous-réseau
        int L = int.Parse(inputs[1]); 
        // E indique le nombre de passerelles du réseau
        int E = int.Parse(inputs[2]);
        // On rempli les veleurs de liens dans le dictionnaire
        foreach(var l in Enumerable.Range(0, L).Select(i => Console.ReadLine().Split()).Select(s => new {From=int.Parse(s[0]), To=int.Parse(s[1])}))
        {
            nodes[l.From].Connections.Add(l.To);
            nodes[l.To].Connections.Add(l.From);
        }
        // On donne implémente le poid des sorties à 1
        foreach(var g in  Enumerable.Range(0, E).Select(i => int.Parse(Console.ReadLine())))
            nodes[g].Weight = 1;
            
        // game loop
        while (true)
        {
            //SI représnte la position de l'agent Skynet
            int SI = int.Parse(Console.ReadLine()); 
            nodes[SI].WeightN = 0;
            //initialise un point d'origine pour le calcul du poid
            var from = SI; 
            
            //on verifie le poid de la position  de skynet
            if(nodes[SI].Weight > 2)
            {
                try
                {
                    //On determine un noeud à coupé en priorisant les liens doubles à proximité du gateway le plus proche
                    from = nodes.Select(x => x.Key).Where(x => nodes[x].Urgence > 0).OrderBy(x=>nodes[x].WeightN).First(); 
                }
                catch(Exception)
                {
                    //Dans le cas ou il n'y a pas de noeud connecté à deux gateway.
                    while(nodes[from].Weight > 2)
                    {
                        from = nodes[from].Connections.OrderBy(x => nodes[x].Weight).First();
                    }
                }
            }
            //On selectionne un gateway connecté au noeud d'origine selctionné précédement
            var to = nodes[from].Connections.OrderBy(x => nodes[x].Weight).First(); 
            Console.WriteLine(from + " " + to);
            //On suprime les liens
            nodes[from].Connections.Remove(to);
            nodes[to].Connections.Remove(from);
            
            // On recalcule les poids
            foreach(var v in nodes.Values)
            {
                v.Urgence = 0;
                if(v.Weight > 1)
                v.Weight = int.MaxValue;
            }
            foreach(var v in nodes.Values.Where(n => n.Weight == 1))
                v.Weight = 1;
        }
    }
    
class Node
    {
        //Liste des noeuds et gateways connecté
        public IList<int> Connections = new List<int>();
        
        //Poid du noeud en fonction de la position du gateway
        private int _WeightG = int.MaxValue;
        
        //Poid du noeud en fonction de la position du SI
        private int _WeightN = int.MaxValue;
        
        //Poid du noeud en fonction du nombre de gateway connecté
        private int _Urgence = 0;
        public int WeightN
        {
            get { return _WeightN; }
            set
            {
                _WeightN = value;
                if (value != int.MaxValue)
                {
                    foreach (var n in Connections.Select(c => nodes[c]).Where(c=>c.Weight==2).Where(c => c.WeightN > value + 1))
                        n.WeightN = value + 1;
                }
            }
        }
        public int Weight
        {
            get { return _WeightG; }
            set
            {
                _WeightG = value;
                if (value != int.MaxValue)
                {
                    foreach (var n in Connections.Select(c => nodes[c]).Where(c => c.Weight > value))
                    {
                        if(n.Weight < 3)
                        {
                            n.Urgence++;
                        }
                        else
                        {
                            n.Weight = value + 1;
                        }
                        
                    }
                }
            }
        }
        public int Urgence
        {
            get { return _Urgence; }
            set
            {
                _Urgence = value;
            }
        }
    }
}

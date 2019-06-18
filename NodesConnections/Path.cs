using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesConnections
{
    class PathFinder
    {
        private List<Node> nodes;

        public PathFinder(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public List<Node> GetPath(Node startNode, Node finishNode)
        {
            List<PathedNode> pathed = new List<PathedNode>();
            foreach (Node n in Global.nodes) {
                PathedNode p = new PathedNode(n);
                p.path = new Dictionary<Node, float>();
                p.visited = false;
                p.UpdateMark();
                pathed.Add(p);
            }

            PathedNode GetNode(Node n)
            {
                return pathed.Where(p => p.node == n).ElementAt(0);
            }

            PathedNode start = GetNode(startNode);
            PathedNode finish = GetNode(finishNode);

            PathedNode current = start;
            do
            {
                // Проходим в цикле все ноды, до которых дотягивается текущая
                KeyValuePair<Node, float> min = new KeyValuePair<Node, float>(null, float.MaxValue);
                /*foreach (KeyValuePair<Node, float> n in current.node.rx)
                {
                    PathedNode nextNode = GetNode(n.Key); // Нода, которую проверяем

                    if (nextNode.visited) continue; // Если мы уже её посетили, то пропускаем
                    

                    //if (min.Key == null) // Если минимальной нет то ставим её
                    //{
                    //    min = n;
                    //    continue;
                    //}

                    if (n.Value < min.Value) // Устанавливаем новую минимальную ноду
                    {
                        min = n;
                    }

                    if (nextNode.mark > current.mark + n.Value// || nextNode.path.Count == 0)
                    )
                        // Устанавливаем новую метку ноде если её текущая больше
                    {
                        nextNode.path = current.path;
                        nextNode.path.Add(nextNode.node, n.Value);
                        nextNode.UpdateMark();
                    }
                }*/
                current.visited = true;

                if (current == finish)
                {
                    List<Node> path = new List<Node>();
                    foreach (KeyValuePair<Node, float> n in finish.path) path.Add(n.Key);
                    return path;
                }

                if (min.Key == null) // Если до нужной ноды не дотянуться вообще
                {
                    return null;
                }

                current = GetNode(min.Key);
            }
            while (true);
        }
        
        private class PathedNode
        {
            public Node node;
            public Dictionary<Node, float> path;
            public float mark = int.MaxValue;
            public bool visited = false;

            public PathedNode(Node n)
            {
                this.node = n;
            }

            public void UpdateMark()
            {
                mark = 0;
                foreach (KeyValuePair<Node, float> n in path)
                {
                    mark += n.Value;
                }
                if (path.Count == 0) mark = float.MaxValue;
            }
        }
    }
}

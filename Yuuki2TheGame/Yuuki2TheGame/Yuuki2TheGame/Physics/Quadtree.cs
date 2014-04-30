﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{
    public class QuadTree<T> where T : class, IQuadObject
    {
        private readonly bool sort;
        private readonly Point minLeafSize;
        private readonly int maxObjectsPerLeaf;
        private QuadNode root = null;
        private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();
        private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
        public QuadNode Root { get { return root; } }
        private object syncLock = new object();
        private int objectSortId = 0;

        public QuadTree(Point minLeafSize, int maxObjectsPerLeaf)
        {
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }

        public int GetSortOrder(T quadObject)
        {
            if (!objectSortOrder.ContainsKey(quadObject))
                return -1;
            else
            {
                return objectSortOrder[quadObject];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLeafSize">The smallest size a leaf will split into</param>
        /// <param name="maxObjectsPerLeaf">Maximum number of objects per leaf before it forces a split into sub quadrants</param>
        /// <param name="sort">Whether or not queries will return objects in the order in which they were added</param>
        public QuadTree(Point minLeafSize, int maxObjectsPerLeaf, bool sort)
            : this(minLeafSize, maxObjectsPerLeaf)
        {
            this.sort = sort;
        }

        public void Insert(T quadObject)
        {
            if (sort & !objectSortOrder.ContainsKey(quadObject))
            {
                objectSortOrder.Add(quadObject, objectSortId++);
            }

            Rectangle bounds = quadObject.Bounds;
            if (root == null)
            {
                var rootSize = new Point((int)Math.Ceiling(bounds.Width / (double) minLeafSize.X),
                                        (int)Math.Ceiling(bounds.Height / (double) minLeafSize.Y));
                double multiplier = Math.Max(rootSize.X, rootSize.Y);
                rootSize = new Point((int)Math.Round(minLeafSize.X * multiplier), (int)Math.Round(minLeafSize.X * multiplier));
                var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
                var rootOrigin = new Point(center.X - rootSize.X / 2, center.Y - rootSize.Y / 2);
                root = new QuadNode(new Rectangle(rootOrigin.X, rootOrigin.Y, rootSize.X, rootSize.Y));
            }

            while (!root.Bounds.Contains(bounds))
            {
                ExpandRoot(bounds);
            }

            InsertNodeObject(root, quadObject);
        }

        public List<T> Query(Rectangle bounds)
        {
            List<T> results = new List<T>();
            if (root != null)
                Query(bounds, root, results);
            if (sort)
                results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
            return results;
        }

        private void Query(Rectangle bounds, QuadNode node, List<T> results)
        {
            if (node == null) return;

            if (bounds.Intersects(node.Bounds))
            {
                foreach (T quadObject in node.Objects)
                {
                    if (bounds.Intersects(quadObject.Bounds))
                        results.Add(quadObject);
                }

                foreach (QuadNode childNode in node.Nodes)
                {
                    Query(bounds, childNode, results);
                }
            }
        }

        private void ExpandRoot(Rectangle newChildBounds)
        {
            bool isNorth = root.Bounds.Y < newChildBounds.Y;
            bool isWest = root.Bounds.X < newChildBounds.X;

            Direction rootDirection;
            if (isNorth)
            {
                rootDirection = isWest ? Direction.NW : Direction.NE;
            }
            else
            {
                rootDirection = isWest ? Direction.SW : Direction.SE;
            }

            double newX = (rootDirection == Direction.NW || rootDirection == Direction.SW)
                                ? root.Bounds.X
                                : root.Bounds.X - root.Bounds.Width;
            double newY = (rootDirection == Direction.NW || rootDirection == Direction.NE)
                                ? root.Bounds.Y
                                : root.Bounds.Y - root.Bounds.Height;
            Rectangle newRootBounds = new Rectangle((int) Math.Round(newX), (int) Math.Round(newY), root.Bounds.Width * 2, root.Bounds.Height * 2);
            QuadNode newRoot = new QuadNode(newRootBounds);
            SetupChildNodes(newRoot);
            newRoot[rootDirection] = root;
            root = newRoot;
        }

        private void InsertNodeObject(QuadNode node, T quadObject)
        {
            if (!node.Bounds.Contains(quadObject.Bounds))
                throw new Exception("This should not happen, child does not fit within node bounds");

            if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
            {
                SetupChildNodes(node);

                List<T> childObjects = new List<T>(node.Objects);
                List<T> childrenToRelocate = new List<T>();

                foreach (T childObject in childObjects)
                {
                    foreach (QuadNode childNode in node.Nodes)
                    {
                        if (childNode == null)
                            continue;

                        if (childNode.Bounds.Contains(childObject.Bounds))
                        {
                            childrenToRelocate.Add(childObject);
                        }
                    }
                }

                foreach (T childObject in childrenToRelocate)
                {
                    RemoveQuadObjectFromNode(childObject);
                    InsertNodeObject(node, childObject);
                }
            }

            foreach (QuadNode childNode in node.Nodes)
            {
                if (childNode != null)
                {
                    if (childNode.Bounds.Contains(quadObject.Bounds))
                    {
                        InsertNodeObject(childNode, quadObject);
                        return;
                    }
                }
            }

            AddQuadObjectToNode(node, quadObject);
        }

        private void ClearQuadObjectsFromNode(QuadNode node)
        {
            List<T> quadObjects = new List<T>(node.Objects);
            foreach (T quadObject in quadObjects)
            {
                RemoveQuadObjectFromNode(quadObject);
            }
        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {
            QuadNode node = objectToNodeLookup[quadObject];
            node.quadObjects.Remove(quadObject);
            objectToNodeLookup.Remove(quadObject);
            quadObject.OnBoundsChanged -= new EventHandler(quadObject_BoundsChanged);
        }

        private void AddQuadObjectToNode(QuadNode node, T quadObject)
        {
            node.quadObjects.Add(quadObject);
            objectToNodeLookup.Add(quadObject, node);
            quadObject.OnBoundsChanged += new EventHandler(quadObject_BoundsChanged);
        }

        void quadObject_BoundsChanged(object sender, EventArgs e)
        {
            T quadObject = sender as T;
            if (quadObject != null)
            {
                QuadNode node = objectToNodeLookup[quadObject];
                if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes())
                {
                    RemoveQuadObjectFromNode(quadObject);
                    Insert(quadObject);
                    if (node.Parent != null)
                    {
                        CheckChildNodes(node.Parent);
                    }
                }
            }
        }

        private void SetupChildNodes(QuadNode node)
        {
            if (minLeafSize.X <= node.Bounds.Width / 2 && minLeafSize.Y <= node.Bounds.Height / 2)
            {
                node[Direction.NW] = new QuadNode(node.Bounds.X, node.Bounds.Y, node.Bounds.Width / 2,
                                                    node.Bounds.Height / 2);
                node[Direction.NE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2, node.Bounds.Y,
                                                    node.Bounds.Width / 2,
                                                    node.Bounds.Height / 2);
                node[Direction.SW] = new QuadNode(node.Bounds.X, node.Bounds.Y + node.Bounds.Height / 2,
                                                    node.Bounds.Width / 2,
                                                    node.Bounds.Height / 2);
                node[Direction.SE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2,
                                                    node.Bounds.Y + node.Bounds.Height / 2,
                                                    node.Bounds.Width / 2, node.Bounds.Height / 2);

            }
        }

        public void Remove(T quadObject)
        {
            if (sort && objectSortOrder.ContainsKey(quadObject))
            {
                objectSortOrder.Remove(quadObject);
            }

            if (!objectToNodeLookup.ContainsKey(quadObject))
                throw new KeyNotFoundException("QuadObject not found in dictionary for removal");

            QuadNode containingNode = objectToNodeLookup[quadObject];
            RemoveQuadObjectFromNode(quadObject);

            if (containingNode.Parent != null)
                CheckChildNodes(containingNode.Parent);
        }



        private void CheckChildNodes(QuadNode node)
        {
            if (GetQuadObjectCount(node) <= maxObjectsPerLeaf)
            {
                // Move child objects into this node, and delete sub nodes
                List<T> subChildObjects = GetChildObjects(node);
                foreach (T childObject in subChildObjects)
                {
                    if (!node.Objects.Contains(childObject))
                    {
                        RemoveQuadObjectFromNode(childObject);
                        AddQuadObjectToNode(node, childObject);
                    }
                }
                if (node[Direction.NW] != null)
                {
                    node[Direction.NW].Parent = null;
                    node[Direction.NW] = null;
                }
                if (node[Direction.NE] != null)
                {
                    node[Direction.NE].Parent = null;
                    node[Direction.NE] = null;
                }
                if (node[Direction.SW] != null)
                {
                    node[Direction.SW].Parent = null;
                    node[Direction.SW] = null;
                }
                if (node[Direction.SE] != null)
                {
                    node[Direction.SE].Parent = null;
                    node[Direction.SE] = null;
                }

                if (node.Parent != null)
                    CheckChildNodes(node.Parent);
                else
                {
                    // Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
                    int numQuadrantsWithObjects = 0;
                    QuadNode nodeWithObjects = null;
                    foreach (QuadNode childNode in node.Nodes)
                    {
                        if (childNode != null && GetQuadObjectCount(childNode) > 0)
                        {
                            numQuadrantsWithObjects++;
                            nodeWithObjects = childNode;
                            if (numQuadrantsWithObjects > 1) break;
                        }
                    }
                    if (numQuadrantsWithObjects == 1)
                    {
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode != nodeWithObjects)
                                childNode.Parent = null;
                        }
                        root = nodeWithObjects;
                    }
                }
            }
        }


        private List<T> GetChildObjects(QuadNode node)
        {
            List<T> results = new List<T>();
            results.AddRange(node.quadObjects);
            foreach (QuadNode childNode in node.Nodes)
            {
                if (childNode != null)
                    results.AddRange(GetChildObjects(childNode));
            }
            return results;
        }

        public int GetQuadObjectCount()
        {
            if (root == null)
                return 0;
            int count = GetQuadObjectCount(root);
            return count;
        }

        private int GetQuadObjectCount(QuadNode node)
        {
            int count = node.Objects.Count;
            foreach (QuadNode childNode in node.Nodes)
            {
                if (childNode != null)
                {
                    count += GetQuadObjectCount(childNode);
                }
            }
            return count;
        }

        public int GetQuadNodeCount()
        {
            if (root == null)
                return 0;
            int count = GetQuadNodeCount(root);
            return count;
        }

        private int GetQuadNodeCount(QuadNode node)
        {
            if (node == null) return 0;
            int count = 1;
            foreach (QuadNode childNode in node.Nodes)
            {
                if (childNode != null)
                    count += GetQuadNodeCount(childNode);
            }
            return count;
        }

        public List<QuadNode> GetAllNodes()
        {
            List<QuadNode> results = new List<QuadNode>();
            if (root != null)
            {
                results.Add(root);
                GetChildNodes(root, results);
            }
            return results;
        }

        private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
        {
            foreach (QuadNode childNode in node.Nodes)
            {
                if (childNode != null)
                {
                    results.Add(childNode);
                    GetChildNodes(childNode, results);
                }
            }
        }

        public class QuadNode
        {
            private static int _id = 0;
            public readonly int ID = _id++;

            public QuadNode Parent { get; internal set; }

            private QuadNode[] _nodes = new QuadNode[4];
            public QuadNode this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            return _nodes[0];
                        case Direction.NE:
                            return _nodes[1];
                        case Direction.SW:
                            return _nodes[2];
                        case Direction.SE:
                            return _nodes[3];
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            _nodes[0] = value;
                            break;
                        case Direction.NE:
                            _nodes[1] = value;
                            break;
                        case Direction.SW:
                            _nodes[2] = value;
                            break;
                        case Direction.SE:
                            _nodes[3] = value;
                            break;
                    }
                    if (value != null)
                        value.Parent = this;
                }
            }

            public ReadOnlyCollection<QuadNode> Nodes;

            internal List<T> quadObjects = new List<T>();
            public ReadOnlyCollection<T> Objects;

            public Rectangle Bounds { get; internal set; }

            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }

            public QuadNode(Rectangle bounds)
            {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
                Objects = new ReadOnlyCollection<T>(quadObjects);
            }

            public QuadNode(int x, int y, int width, int height)
                : this(new Rectangle(x, y, width, height))
            {

            }
        }
    }

    public enum Direction : int
    {
        NW = 0,
        NE = 1,
        SW = 2,
        SE = 3
    }
}
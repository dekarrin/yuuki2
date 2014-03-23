using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{

    class QuadTree
    {

        private class QuadNode
        {
            private IList<ICollidable> _items = new List<ICollidable>();

            private QuadNode _upRight = null;

            private QuadNode _upLeft = null;

            private QuadNode _downRight = null;

            private QuadNode _downLeft = null;

            private readonly int _capacity;

            private readonly int _depth;

            private readonly int _maxDepth;

            /// <summary>
            /// Position and size of this QuadNode.
            /// </summary>
            private Rectangle _rect;

            public bool IsLeaf { get; set; }

            public QuadNode(Rectangle location, int capacity, int depth, int maxDepth) {
                _capacity = capacity;
                _depth = depth;
                _maxDepth = maxDepth;
            }

            private void Split()
            {
                int mw = _rect.Width / 2;
                int mh = _rect.Height / 2;
                int mx = _rect.X + mw;
                int my = _rect.Y + mh;
                Rectangle nw = new Rectangle(_rect.X, _rect.Y, mw, mh);
                Rectangle ne = new Rectangle(mx, _rect.Y, _rect.Width - mw, mh);
                Rectangle sw = new Rectangle(_rect.X, my, mw, _rect.Height - mh);
                Rectangle se = new Rectangle(mx, my, _rect.Width - mw, _rect.Height - mh);
                _upRight = new QuadNode(ne, _capacity, _depth + 1, _maxDepth);
                _upLeft = new QuadNode(nw, _capacity, _depth + 1, _maxDepth);
                _downRight = new QuadNode(se, _capacity, _depth + 1, _maxDepth);
                _downLeft = new QuadNode(sw, _capacity, _depth + 1, _maxDepth);
                IList<ICollidable> toRemove = new List<ICollidable>();
                foreach (ICollidable ic in _items)
                {
                    if (ic.BoundingBox.X < mx)
                    {
                        if (ic.BoundingBox.Y < my && nw.Contains(ic.BoundingBox))
                        {
                            _upLeft.Insert(ic);
                            toRemove.Add(ic);
                        }
                        else if (sw.Contains(ic.BoundingBox))
                        {
                            _downLeft.Insert(ic);
                            toRemove.Add(ic);
                        }
                    }
                    else
                    {
                        if (ic.BoundingBox.Y < my && ne.Contains(ic.BoundingBox))
                        {
                            _upRight.Insert(ic);
                            toRemove.Add(ic);
                        }
                        else if (se.Contains(ic.BoundingBox))
                        {
                            _downRight.Insert(ic);
                            toRemove.Add(ic);
                        }
                    }
                }
                foreach (ICollidable ic in toRemove)
                {
                    _items.Remove(ic);
                }
            }

            public void Insert(ICollidable item)
            {
                if (IsLeaf)
                {
                    _items.Add(item);
                    if (_items.Count > _capacity && _depth < _maxDepth)
                    {
                        Split();
                    }
                }
                else
                {
                    QuadNode quad = GetQuad(item);
                }
            }

            public void Remove(ICollidable item)
            {
                QuadNode quad = GetQuad(item);
                quad.Remove(item);
            }

            public QuadNode GetQuad(ICollidable item)
            {
                if (_items.Contains(item))
                {
                    return this;
                }
                int mw = _rect.Width / 2;
                int mh = _rect.Height / 2;
                int mx = _rect.X + mw;
                int my = _rect.Y + mh;
                if (item.BoundingBox.X < mx)
                {
                    if (item.BoundingBox.Y < my && _upLeft._rect.Contains(ic.BoundingBox))
                    {
                        return _upLeft.GetQuad(item);
                    }
                    else if (_downLeft._rect.Contains(item.BoundingBox))
                    {
                        return _downLeft.GetQuad(item);
                    }
                }
                else
                {
                    if (item.BoundingBox.Y < my && _upRight._rect.Contains(item.BoundingBox))
                    {
                        return _upRight.GetQuad(item);
                    }
                    else if (_downRight._rect.Contains(item.BoundingBox))
                    {
                        return _downRight.GetQuad(item);
                    }
                }
                return null;
            }

            public IList<ICollidable> Query(Rectangle rect)
            {
                return null;
            }

        }

    }
}

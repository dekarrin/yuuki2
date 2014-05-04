using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Physics
{

    enum DragShape
    {
        SPHERE,
        HALF_SPHERE,
        CONE,
        CUBE,
        ANGLED_CUBE,
        LONG_CYLINDER,
        SHORT_CYLINDER,
        STREAMLINED_BODY,
        STREAMLINED_HALF_BODY,
    }

    /// <summary>
    /// Simplified model! Assumes that our broad side is always facing the direction of movement!
    /// </summary>
    class DragModel
    {

        public DragModel(DragShape type)
        {
            Shape = type;
            switch (type)
            {
                case DragShape.SPHERE:
                    Coefficient = 0.47f;
                    break;

                case DragShape.HALF_SPHERE:
                    Coefficient = 0.42f;
                    break;

                case DragShape.CONE:
                    Coefficient = 0.50f;
                    break;

                default:
                case DragShape.CUBE:
                    Coefficient = 1.05f;
                    break;

                case DragShape.ANGLED_CUBE:
                    Coefficient = 0.80f;
                    break;

                case DragShape.LONG_CYLINDER:
                    Coefficient = 0.82f;
                    break;

                case DragShape.SHORT_CYLINDER:
                    Coefficient = 1.15f;
                    break;

                case DragShape.STREAMLINED_BODY:
                    Coefficient = 0.04f;
                    break;

                case DragShape.STREAMLINED_HALF_BODY:
                    Coefficient = 0.09f;
                    break;

            }
        }

        public DragShape Shape { get; private set; }

        public float Coefficient { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.drawObjects {
    // 10mm = 1px 
    public class drawObject {
        public RectangleF loc;
        public Graphics g;
        public Pen p = Pens.Black;
		public RectangleF origLoc;

        public drawObject(int x, int y, int w, int h) {
            this.loc = new RectangleF(x, y, w, h);
			this.origLoc = new RectangleF(x, y, w, h);
        }

        virtual public void draw(Graphics g) { }

        //Returns if it should be redrawn
        virtual public bool timer(int ms) { return false; }

        virtual public void checkCollision(RectangleF toCheck, Graphics p, string Name) { }

		// Reset to center of drawing.
		public void reset() {
			this.loc = new RectangleF(this.origLoc.X, this.origLoc.Y, this.origLoc.Width, this.origLoc.Height);
			this._reset();
		}

		virtual public void _reset() { }
    }
}

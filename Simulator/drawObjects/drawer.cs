using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator.drawObjects {

	class drawer {
		private PictureBox pbParent;
		private Dictionary<String, drawObject> drawObjects = new Dictionary<string, drawObject>();

		public drawer(ref PictureBox newParent) {
			this.pbParent = newParent;
			this.pbParent.Paint += paintEvent;
		}

		public void addToDrawer(string key, drawObject toAdd) {
			this.drawObjects.Add(key, toAdd);
		}

		public drawObject get(string key) {
			if (this.drawObjects.ContainsKey(key)) {
				return this.drawObjects[key];
			} else {
				return null;
			}
		}

        private void checkCollisions() {
            foreach (KeyValuePair<String, drawObject> item in drawObjects) {
				foreach (KeyValuePair<String, drawObject> itemTwo in drawObjects) {
					if (!item.Equals(itemTwo)) {
                        item.Value.checkCollision(itemTwo.Value.loc, this.pbParent.CreateGraphics(), itemTwo.Key);
                    }
				}
			}
		}

		private void doDraw(Graphics g) {

            Region baseR = new Region(new Rectangle(0, 0, this.pbParent.Width, this.pbParent.Height));

            foreach (KeyValuePair<String, drawObject> item in drawObjects) {
                if (item.Key.Substring(0, 5) == "table") {
                    baseR.Exclude(Rectangle.Round(item.Value.loc));
                }
            }
            
            g.FillRegion(new SolidBrush(this.pbParent.BackColor), baseR);
         
			foreach (KeyValuePair<String, drawObject> item in drawObjects) {
				item.Value.draw(g);
			}
		}

		private void paintEvent(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;
			this.doDraw(g);
		}

		public void timer(int mSec) {
			if (mSec == 100) {
				this.checkCollisions();
			}
			bool doRedraw = false;
			foreach (KeyValuePair<String, drawObject> item in drawObjects) {
				if (item.Value.timer(mSec)) {
					doRedraw = true;
				}
			}
			if (doRedraw) {
				doDraw(this.pbParent.CreateGraphics());
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;

namespace Simulator.drawObjects {

	class drawTable : drawObject {
		private const int widthInPx = 260;
		private const int heightInPx = 260;
	//	private new Pen p = Pens.DarkBlue;

		private Brush b = Brushes.DarkBlue;

		public drawTable(int x, int y)
			: base(x, y, widthInPx, heightInPx) {

		}

		public override void draw(Graphics g) {
		//	g.DrawRectangle(this.p, Rectangle.Round(base.loc));
			g.FillRectangle(this.b, Rectangle.Round(base.loc));
		}
	}

    class drawPool : drawObject {
        private const int widthInPx = 150;
        private const int heightInPx = 3000;
        private Brush b = Brushes.Aquamarine;

        public drawPool(int x, int y)
            : base(x, y, widthInPx, heightInPx) {

        }

        public override void draw(Graphics g) {
            g.FillRectangle(this.b, Rectangle.Round(base.loc));
        }
    }

    class drawDock : drawObject {
        private const int widthInPx = 80;
        private const int heightInPx = 80;
        private Brush b = Brushes.Aquamarine;

		Bitmap image1 = (Bitmap)Properties.Resources.dock;


        public drawDock(int x, int y)
            : base(x, y, widthInPx, heightInPx) {

        }

        public override void draw(Graphics g) {

            TextureBrush texture = new TextureBrush(image1);
            texture.WrapMode = System.Drawing.Drawing2D.WrapMode.Tile;

            g.FillEllipse(texture, Rectangle.Round(base.loc));
        }
    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Simulator.drawObjects {

	public class drawRoomba : drawObject {

		private const int roombaWidthInPx = 26;
		private const int roombaHeightInPx = 26;
		private const double WHEELBASE = 25.8;
		private const int WHEELWIDTH = 10;
		private const double TIMEFACTOR = 0.01;
		private const bool drawPath = true;
		private const int MAXLENGTHOFPATH = 250000;

		struct wheel {
			public double speed;
			public PointF pos;
			public Pen p;
		}

        #region delegates
        private static void sensorDummy(int sensorNr, byte[] values) { }
        public delegate void sensorDelegate(int sensorNr, byte[] values);
        private sensorDelegate sensor = new sensorDelegate(sensorDummy);
        public void setSensorFunction(sensorDelegate sensorFunction) {
            sensor += new sensorDelegate(sensorFunction);
        }
        #endregion

        #region variables
        private PointF centerPoint;
		private List<PointF> paths;
		private new Pen p = Pens.Blue;
		private double angle;
		private RectangleF innerLoc;
		private bool isDriving = false;
        private bool isColliding = false;
        private bool isCharging = false;
        private bool isFalling = false;
		private string interactionObject = "";
		private wheel leftWheel;
		private wheel rightWheel;
		private PointF wheelPos;
        #endregion

        public drawRoomba(int x, int y, int angle)
			: base(x, y, roombaWidthInPx, roombaHeightInPx) {
			this.innerLoc = new RectangleF(base.loc.X + 2, base.loc.Y + 2, roombaWidthInPx - 4, roombaHeightInPx - 4);
			this.angle = angle;
			this.leftWheel.pos.X = x;
			this.leftWheel.pos.Y = y + (roombaHeightInPx / 2);
			this.leftWheel.p = Pens.Green;

			this.rightWheel.pos.X = x + roombaWidthInPx;
			this.rightWheel.pos.Y = y + (roombaHeightInPx / 2);
			this.rightWheel.p = Pens.Red;

			this.wheelPos = degreesToXY(this.angle + 90, WHEELWIDTH);

			this.centerPoint.X = base.loc.X + (base.loc.Width / 2);
			this.centerPoint.Y = base.loc.Y + (base.loc.Height / 2);


			this.paths = new List<PointF>();
		}

		public override void draw(Graphics g) {

            #region drawRoombaWorksDontTouch
            g.DrawArc(this.p, base.loc, 0, 360);
			g.DrawArc(this.p, this.innerLoc, (int)(this.angle - 90) - 22, 45);
			if (drawPath) {
				if (this.paths.Count > 1) {
					g.DrawLines(Pens.Red, this.paths.ToArray());
				}
            }
            #endregion

		}

		enum direction {
			left = 0x00,
			right,
			top,
			bottom
		};

        public override void checkCollision(RectangleF toCheck, Graphics g, string name) {
            
            Pen debugPen = Pens.Purple;
			RectangleF collisionArea = RectangleF.Intersect(base.loc, toCheck);

            double a = this.angle % 360;
			double posAngle = a;
			if (a < 0) {
				posAngle += 360;
			}
			a = Math.Abs(a);
            Boolean emerge = (collisionArea.Width == base.loc.Width && collisionArea.Height == base.loc.Height);

			
           
            if (!collisionArea.IsEmpty) {
				PointF collisionCenter = new PointF(collisionArea.X + (collisionArea.Width / 2), collisionArea.Y + (collisionArea.Height / 2));

                // shows collisions on screen;
                g.DrawRectangle(debugPen, Rectangle.Round(collisionArea));

                double toRadian = (Math.PI / 180.0);
                double toDegree = (180.0 / Math.PI);
				direction dir;
				

				//Calculate triangle
				double overstaand = collisionCenter.Y - this.centerPoint.Y;
				double aanliggend = collisionCenter.X - this.centerPoint.X;

                if (collisionArea.Width > collisionArea.Height) {
					overstaand = collisionCenter.X - this.centerPoint.X;
					aanliggend = collisionCenter.Y - this.centerPoint.Y;
					

					
                    double invAngle = ((double)180.0 - a);
                    double sin = Math.Sin(invAngle * toRadian);
                    double sz = (WHEELBASE / 2.0) / sin;
                    a = Math.Acos((WHEELBASE/2)/sz)*toDegree;
                }

				
				double origA = a;
				if (posAngle < 135) {
					if (a > 180) {
						a -= 180;
					}
				} else if (posAngle > 315) {
					//a = 180 - a;
				}
				
                int returning = 0;

                if (name.Substring(0,4) == "pool") {

                    byte[] bytes = { (byte)1 };

                    if (a >= 0 && a <= 45) {
                        sensor(9, bytes);
                    }
                    if (a >= 45 && a <= 90) {
                        sensor(10, bytes);
                    }
                    if (a >= 90 && a <= 135) {
                        sensor(11, bytes);
                    }
                    if (a >= 135 && a <= 180) {
                        sensor(12, bytes);
                    }

                    if (emerge) {
                        bytes[0] = (byte)(8+4);
                        sensor(7, bytes);
                        this.isColliding = true;
                    }

                    this.isFalling = true;

				} else if (name.Substring(0, 5) == "table") {

					if ((posAngle < 270) || posAngle < 135) {
						if (a >= 0 && a <= 112.5) {
							returning += 2;
						}
						if (a >= 67.5 && a <= 180) {
							returning += 1;
						}
					}else{

					//if (posAngle > 315 || (posAngle > 135 && posAngle < 180)) {
						if (a >= 0 && a <= 112.5) {
							returning += 1;
						}
						if (a >= 67.5 && a <= 180) {
							returning += 2;
						}
					}

                    byte[] bytes = { (byte)returning };
                    sensor(7, bytes);

                    if (!this.isColliding) {
                        Debug.WriteLine(String.Format("Roomba angle: {0}; Impact angle before: {3}; Impact angle: {1}; PosAngle: {2};", (int)this.angle, (int)a, (int) posAngle, (int) origA));
                    }

					this.interactionObject = name;
                    this.isColliding = true;

				} else if (name.Substring(0, 4) == "dock") {

                    int charge;

                    if (emerge) {
                        charge = 2;
                        this.isCharging = true;
                    } else {
                        charge = 3;
                    }


                    byte[] bytes = { (byte)charge };
                    sensor(21,bytes);

                }

            } else {


                byte[] bytes = { (byte)0 };

                if (name.Substring(0,4) == "pool") {

					sensor(9, bytes);
					sensor(10, bytes);
					sensor(11, bytes);
					sensor(12, bytes);
					this.isFalling = false;

				} else if (name.Substring(0, 5) == "table") {

					if ((name == this.interactionObject) && this.isColliding) {
						sensor(7, bytes);
						this.isColliding = false;
					}

				} else if (name == "dock") {

					sensor(21, bytes);
					this.isCharging = false;

                }


            }

		}

		public override void _reset() {
			this.setSpeed(0, 0);
			this.centerPoint.X = base.loc.X;
			this.centerPoint.Y = base.loc.Y;
			calculateTravelDistance();
		}

		private static PointF degreesToXY(double degrees, double radius) {
			double dividor = Math.PI / 180.0;
			double radians = degrees * dividor;
			PointF result = new PointF();
			result.X = (float) (Math.Cos(radians) * radius);
			result.Y = (float) (Math.Sin(radians) * radius);
			return result;
		}

		private void doAdd(ref PointF target, PointF toAdd) {
			target.X += toAdd.X;
			target.Y += toAdd.Y;
		}

		private void calculateTravelDistance() {
			//Calculate the angle at which the roomba is turning. angleAdj.
			//We know that the difference between the wheels will give us a
			//Triangle with equivalent legs
			//When we aply some magic formulas to that(see below)
			//We get the angle the roomba has rotated

			//Now, we calculate the position that one of the wheels move
			double leftWheelSpeed = Math.Abs(leftWheel.speed);
			double rightWheelSpeed = Math.Abs(rightWheel.speed);

			double rotationPointOnAxis = 0;
			double smallestSpeedAbs = (leftWheelSpeed < rightWheelSpeed ? leftWheelSpeed : rightWheelSpeed);
			double biggestSpeedAbs = (smallestSpeedAbs == leftWheelSpeed ? rightWheelSpeed : leftWheelSpeed);

			if (leftWheelSpeed != rightWheelSpeed) {
				double negativeOrPositive = (rightWheelSpeed > leftWheelSpeed ? -1 : 1);
				rotationPointOnAxis = negativeOrPositive * (100 - ((smallestSpeedAbs / biggestSpeedAbs) * 100));
			}

			rotationPointOnAxis = (double) ( ( ( (double)rotationPointOnAxis + 100) / 200) * WHEELBASE);

			double distanceTraveled = ((leftWheel.speed + rightWheel.speed) / 2); //Divided by 100 because speed is in seconds, and this function is in 0.01 seconds
			

			double dDistance = leftWheel.speed - rightWheel.speed;
			//Calculate the angle at which the roomba is turning. angleAdj.
			//We know that the difference between the wheels will give us a
			//Triangle with equivalent legs
			//When we aply some magic formulas to that(see below)
			//We get the angle the roomba has rotated
			double d = dDistance / 2;
			double f = (d / WHEELBASE);
			double sinCalc = Math.Asin(f);
			double angleAdj = ((sinCalc / Math.PI) * 180);
			angleAdj = angleAdj * 2;

			//We then add that to the global angle
			this.angle += angleAdj;

			//Basespeed + diff and angle
			//Centerpoint stays on the same spot on the axis
			//This means that it travels the distance that both wheels travel on their own
			//Including the HALF of the distance the fastest wheel moves away.
			
			PointF newPoint = degreesToXY((float)this.angle - 90, distanceTraveled);

			doAdd(ref this.centerPoint, newPoint);
			base.loc.X = this.centerPoint.X - (base.loc.Width / 2);
			base.loc.Y = this.centerPoint.Y - (base.loc.Height / 2);

			//Add the current location to a list.
			//That way we can see where we have been :D
			//YAY :D
			if (this.paths.Count == 0) {
				//Always add
				this.paths.Add(centerPoint);
			} else if (this.paths.Count > 0) {
				if (!this.paths[this.paths.Count - 1].Equals(centerPoint)) {
					this.paths.Add(centerPoint);
				}

				if (this.paths.Count > MAXLENGTHOFPATH) {
					this.paths.RemoveAt(0);
				}
			}

			//Innerloc is a rectangle which holds a smaller circle.
			//This circle shows you which direction the roomba is heading
			this.innerLoc = new RectangleF(base.loc.X + 2, base.loc.Y + 2, roombaWidthInPx - 4, roombaHeightInPx - 4);

			//wheelPos will hold a small line that shows the direction of the wheels
			this.wheelPos = degreesToXY(this.angle + 90, WHEELWIDTH);
		}

		public override bool timer(int ms) {
			bool doRedraw = false;
			if (ms == 500) {

			} else if (ms == 10) {
                doRedraw = ( this.isDriving && !this.isColliding && !this.isCharging ) || ( leftWheel.speed < 0 || rightWheel.speed < 0);
                if (doRedraw) {
					calculateTravelDistance();
				}
			}
			return doRedraw;
		}

		public void setSpeed(int left, int right) {
			isDriving = (left != 0 || right != 0);
			this.leftWheel.speed = ((double)left / 10) *TIMEFACTOR; // Divided by 10 for the scale
			this.rightWheel.speed = ((double)right / 10) *TIMEFACTOR;
		}
	}
}

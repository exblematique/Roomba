using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulator.drawObjects;
using System.Diagnostics;

namespace Simulator {

    enum logTags {
        program = ConsoleColor.Green,
        serial = ConsoleColor.Blue,
        serialServer,
        roomba = ConsoleColor.Yellow,
        error = ConsoleColor.Red
    }

    public partial class frmMain : Form {

        #region Variables
        private static serialServer serSock;
        private static clsRoomba roomba;

        static byte command = 0;
        static byte byteCount = 0;
        List<byte> dataBytes = new List<byte>();

		private drawer mDrawer;
        #endregion 

        #region logging
        static void log(string x, logTags tag) {
            string lTag = "";
            if (tag == logTags.program) {
                lTag = "PROG";
            } else if (tag == logTags.serialServer) {
                lTag = "SERI";
            } else if (tag == logTags.serial) {
                lTag = "BYTE";
            } else if (tag == logTags.roomba) {
                lTag = "RMBA";
            }
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)tag;
            //Console.Write(String.Format("[{0}]\t", lTag));
            Console.WriteLine(String.Format("[{0}]\t{1}", lTag, x));
            Console.ForegroundColor = oldColor;
        }
        #endregion

        #region Draw

        static void setSensor(int sensorNr, byte[] values) {

			//log(String.Format("{0}",values[0]), logTags.roomba);

            switch (sensorNr) {
                case 7:
                case 9:
                case 10:
                case 11:
                case 12:
                case 21:
                    roomba.setSensorValue(sensorNr,(int)values[0]);
                    break;
                case 1337:
                    roomba.uncontrolledTest(-1050, -1000);
                    break;
                default:
                    break;
            }

        }

		private void initDrawers() {
			mDrawer = new drawer(ref this.pbRoom);
            drawRoomba roombaDrawer = new drawRoomba((pbRoom.Width / 6)*2, pbRoom.Height / 2, -90);
            roombaDrawer.setSensorFunction(setSensor);
            mDrawer.addToDrawer("roomba", roombaDrawer);

			mDrawer.addToDrawer("table-1", new drawTable(0, 260));

			mDrawer.addToDrawer("table-2-1", new drawTable(260, 0));
            mDrawer.addToDrawer("table-2-2", new drawTable(520, 0));
            mDrawer.addToDrawer("table-2-3", new drawTable(780, 0));

			mDrawer.addToDrawer("table-3", new drawTable(1040, 260));

			mDrawer.addToDrawer("table-4-1", new drawTable(260, 520));
            mDrawer.addToDrawer("table-4-2", new drawTable(520, 400));
            mDrawer.addToDrawer("table-4-3", new drawTable(780, 520));



       //     mDrawer.addToDrawer("pool_1", new drawPool(550, 400));
        //    mDrawer.addToDrawer("dock_1", new drawDock(800, 200));
        }
        #endregion

        #region Form events
        public frmMain() {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            //Load serial server:
            serSock = new serialServer();
            serSock.setLogFunction(log);
            serSock.setMessageHandler(messageHandlerSocket);

            roomba = new clsRoomba(log, send);

            this.initDrawers();

            //roomba.uncontrolledTest(2000, 2000);

            // DEFAULT, since Koen can't divide by 0. Sucker... ;) ============= \\
            roomba.setSensorValue(24, 15);
            roomba.setSensorValue(25, 65535);
            roomba.setSensorValue(26, 65535);
            // ============= \\

        }

        public void listEngines(bool side, bool vacuum, bool main){

            string[] row0 = { "Side Brush", side ? "On" : "Off" };
            string[] row1 = { "Vacuum", vacuum ? "On" : "Off" };
            string[] row2 = { "Main Brush", main ? "On" : "Off" };

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(row0);
            dataGridView1.Rows.Add(row1);
            dataGridView1.Rows.Add(row2);
           
        }

		private void resetRoombaToCenterToolStripMenuItem_Click(object sender, EventArgs e) {
			/*if (drawObjects.ContainsKey("roomba")) {
				drawObjects["roomba"].reset();
				roomba.driveDirect(0, 0, 0, 0);
				doDraw(this.pbRoom.CreateGraphics());
			}*/
		}

        private void tim100_Tick(object sender, EventArgs e) {
            this.mDrawer.timer(100);
            bool[] bools = roomba.getEnginestates();
            listEngines(bools[0], bools[1], bools[2]);
        }

		private void tim1000_Tick(object sender, EventArgs e) {
			this.mDrawer.timer(1000);
		}

        private void tim10_Tick(object sender, EventArgs e) {
			if (this.mDrawer.get("roomba") != null) {
				((drawRoomba)this.mDrawer.get("roomba")).setSpeed(roomba.getSpeed()[0], roomba.getSpeed()[1]);
			}
			this.mDrawer.timer(10);
        }

        private void frmMain_Load(object sender, EventArgs e) {
            //Find the serial port names
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo()) {
                serialPortToolStripMenuItem.Items.Add(comPort.Description);
            }

            // HACK!
            if (serialPortToolStripMenuItem.Items.Count == 1) {
                serialPortToolStripMenuItem.SelectedIndex = 0;
                connectToolStripMenuItem_Click(sender, e);
            // Nog een!
            } else if (serialPortToolStripMenuItem.Items.Count == 2) {
                serialPortToolStripMenuItem.SelectedIndex = 1;
                connectToolStripMenuItem_Click(sender, e);
            }

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (serSock.isConnected()) {
                serSock.disconnect();
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e) {
            String selectedPort = "";
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo()) {
                if (comPort.Description == serialPortToolStripMenuItem.SelectedItem.ToString()) {
                    selectedPort = comPort.Name;
                }
            }

            serSock.init(selectedPort, Properties.Settings.Default.baudRate);

            try {
                serSock.connect();
                this.tsLblStatus.Text = "Status: Connected";
            } catch (serialException exc) {
                log(String.Format("ERROR: {0}\r\nStack{1}", exc.Message, exc.StackTrace), logTags.serialServer);
            }
        }

        private void serialPortToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        #endregion

        #region Serial
		static void send(byte[] bytes, bool doLog = true) {
            serSock.send(bytes, doLog);
        }
        void clearRegisters() {
            command = 0;
            byteCount = 0;
            dataBytes.Clear();
        }

        private void messageHandlerSocket(byte bRead, byte prevByte) {

            byte[,] mainCommands = { /* {command,databytes} */
                                     {128,0}
                                    ,{129,1}
                                    ,{130,0}
                                    ,{131,0}
                                    ,{132,0}
                                    ,{133,0}
                                    ,{134,0}
                                    ,{135,0}
                                    ,{136,0}
                                    ,{137,4}
                                    ,{145,4}
                                    ,{138,1}
                                    ,{144,3}
                                    ,{146,4}
                                    ,{139,3}
                                    ,{140,2}
                                    ,{142,1}
                                    ,{148,1}
                                    ,{149,1}
                                    ,{150,1}
                                   };

            byte[] storeTemp = {0,0};

            try {

                // decide: is this a mainCommand or a databyte
                if (command == 0) {

                    for (int i = 0; i < mainCommands.Length / 2; i++) {
                        if (mainCommands[i, 0] == bRead) {
                            command = mainCommands[i, 0];
                            byteCount = mainCommands[i, 1];
							if(bRead != 149){
								log(String.Format("Main command received: {0}", bRead), logTags.program);
							}
                            break; // no need to keep looping
                        }
                    }

                } else {

                    dataBytes.Add(bRead);
                    //log(String.Format("Databyte received: {0}", bRead), logTags.program);

                    // dynamic byte count
                    if (command == 140 && dataBytes.Count == 2) {
                        byteCount = (byte)((dataBytes[1]*2) + (byte)2);
                    } else if ((command == 148 || command == 149 ) && dataBytes.Count == 1) {
                        byteCount = (byte)(dataBytes[0] + (byte)1);
                    }

                }

                if (dataBytes.Count == byteCount) {

                    switch (command) {
                        case 128: roomba.start(); break;
                        case 130: // catch this one, set to safe.
                        case 131: roomba.safe(); break;
                        case 132: roomba.full(); break;
                        case 133: roomba.power(); break;
                        case 134: roomba.spot(); break;
                        case 135: roomba.clean(); break;
                        case 136: roomba.max(); break;
                        case 137: roomba.drive(dataBytes[0], dataBytes[1], dataBytes[2], dataBytes[3]); break;
                        case 145: roomba.driveDirect(dataBytes[0], dataBytes[1], dataBytes[2], dataBytes[3]); break;
                        case 138: roomba.motors(dataBytes[0]); break;
                        case 144: roomba.pwmMotors(dataBytes[0], dataBytes[1], dataBytes[2]); break;
                        case 146: roomba.drivePwm(dataBytes[0], dataBytes[1], dataBytes[2], dataBytes[3]); break;
                        case 139: roomba.leds(dataBytes[0], dataBytes[1], dataBytes[2]); break;
                        case 140: storeTemp[0] = dataBytes[0];
                                  storeTemp[1] = dataBytes[1];
                                  dataBytes.RemoveRange(0, 2);
                                  roomba.song(storeTemp[0], storeTemp[1], dataBytes.ToArray());
                                  break;
                        case 142: roomba.getSensor(dataBytes[0]); break;
                        case 148: storeTemp[0] = dataBytes[0];
                                  dataBytes.RemoveRange(0, 1);
                                  roomba.startStream(storeTemp[0], dataBytes.ToArray());
                                  break;
                        case 149: storeTemp[0] = dataBytes[0];
                                  dataBytes.RemoveRange(0, 1);
                                  roomba.querySensor(storeTemp[0], dataBytes.ToArray());
                                  break;
                        case 150: roomba.pauseResumeStream(dataBytes[0]); break;
                        case 129: // baud is not implemented
                        default: break;
                    }

                    clearRegisters();
                }

            } catch (clsRoomba.notInCorrectMode excep) {

                log(String.Format("Not in correct mode when calling function {0}", excep.TargetSite.Name), logTags.program);
                clearRegisters();

            }

        }

        #endregion

        private void spotToolStripMenuItem_Click(object sender, EventArgs e) {
            roomba.setSensorValue(18, 2);
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e) {
            roomba.setSensorValue(18, 1);
        }

        private void dockToolStripMenuItem_Click(object sender, EventArgs e) {
            roomba.setSensorValue(18, 4);
        }

        private void sendSensorsToolStripMenuItem_Click(object sender, EventArgs e) {
            byte[] bytes = { 
                 (byte)7
                ,(byte)9
                ,(byte)10
                ,(byte)11
                ,(byte)12
                ,(byte)19
                ,(byte)20
                ,(byte)21
                ,(byte)24
                ,(byte)25
                ,(byte)26
                ,(byte)35
                //,(byte)18
            };

            roomba.querySensor(1, bytes);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;

namespace Simulator {
    enum commandEnum {
        changeStruct = 1,
        getStruct,
        upOrDownloadStruct,
        setAnimation,
        getAnimation,
        LEDtest,
        stopLEDTest,
        ack,
        tack,
        startByte = 255
    }

    #region "Errors"
    [Serializable]
    public class portNotFoundException: Exception {
        public portNotFoundException() : base() { }
    }

    [Serializable]
    public class cannotOpenPortException: Exception {
        public cannotOpenPortException(string msg) : base() { }
    }

    [Serializable]
    public class serialException : Exception {
        public serialException(Exception excep) : base() { }
    }

    [Serializable]
    public class notInitializedException : Exception {
        public notInitializedException() : base() { }
    }
    #endregion

    class serialServer {
        private SerialPort port;
        private string serialPort;
        private int baudRate;

        private bool keepGoing;
        private bool isInitialized;

        public delegate void logDelegate(String x, logTags tag);
        private logDelegate log = new logDelegate(logDummy);

        public delegate void serialMsgReceivedDelegate(byte bRead, byte prevByte);
        private serialMsgReceivedDelegate msgReceived = new serialMsgReceivedDelegate(msgReceivedDummy);

        #region "Dummies"
        private static void logDummy(string x, logTags tag) { }
        private static void msgReceivedDummy(byte bRead, byte prevByte) { }
        #endregion

        private Thread serialThread;

        // <summary>
        /// Sets the log function.
        /// </summary>
        /// <param name="newLogFunc">The new log function.</param>
        public void setLogFunction(logDelegate newLogFunc) {
            log += new logDelegate(newLogFunc);
        }

        /// <summary>
        /// Sets the message handler.
        /// </summary>
        /// <param name="newFunc">The new message handler.</param>
        public void setMessageHandler(serialMsgReceivedDelegate newFunc) {
            msgReceived += new serialMsgReceivedDelegate(newFunc);
        }


        public bool isConnected() {
            return keepGoing;
        }

        public void init(string newPort, int nBaudRate) {
            serialPort = newPort;
            baudRate = nBaudRate;
            try {
                port = new SerialPort(serialPort);
                port.BaudRate = baudRate;
            } catch (IOException exception) {
                //See if port can be found or other exception
                if (SerialPort.GetPortNames().Contains(newPort)) {
                    throw new cannotOpenPortException(exception.Message);
                } else {
                    throw new portNotFoundException();
                }
            } finally {
                log(String.Format("Server initialized on {0} with {1} bps", serialPort, baudRate), logTags.serialServer);
                isInitialized = true;
            }
        }

        public void connect() {
            if (isInitialized) {
                bool canConnect = true;
                try {
                    port.Open();
                } catch (Exception exception) {
                    canConnect = false;
                    throw new serialException(exception);

                } finally {
                    if (canConnect) {
                        keepGoing = true;
                        serialThread = new Thread(listener);
                        serialThread.Start();
                        log(String.Format("Server connected on {0}", serialPort), logTags.serialServer);
                    }

                }
            } else {
                throw new notInitializedException();
            }
        }

        public void disconnect() {
            port.Close();
            keepGoing = false;
        }

		public void send(byte[] buff, bool doLog = true) {
			if (doLog) {
				string t = String.Format("Send({0}): ", buff.Length);
				foreach (byte b in buff) {
					t = String.Format("{0} {1}", t, (int)b);
				}
				log(t, logTags.serial);
			}
            
            try {
                port.Write(buff, 0, buff.Length);
            } catch {
                // no port open, no problem.
            }
        }


        private void listener() {
            byte bRead = 0;
            byte prevByte = 0;

            while (keepGoing) {
                prevByte = bRead;
                try {
                    bRead = (byte) port.ReadByte();
                } catch (System.IO.IOException) {
                    //Probably ended
                    //Don't do a thing
                    break;
                }
                msgReceived(bRead, prevByte);
            }

        }
    }
}

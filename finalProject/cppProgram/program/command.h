#ifndef COMMAND_H
#define COMMAND_H

#include <cstdint>


#include "mqttStop.h"
#include "../lib/SerialLink/SerialLink.h"
#include "OpenInterfaceConfig.h"


/**
 *  \class Command defines all actions available for the Roomba and the current state of roomba
 **/
class command
{
	public:
        command();
		~command();

		const static void run(int16_t speed);
		static void square(int16_t length);
        const static void sensor(int16_t speed);
        void stop();
        const bool getState();
        void changeState();
        
    private:
        static bool running;
        static SerialLink *sl;
        static mqttStop *mq;
        static OIC *oic;
};

#ifdef MQTT_H
#else
#ifdef MAIN
#else 
    SerialLink* command::sl = new SerialLink("/dev/ttyUSB0", 115200);
    bool command::running = false;
    mqttStop* command::mq = new mqttStop("SignalStop", "localhost", 1883);
    OIC* command::oic = new OIC();
#endif
#endif

#endif
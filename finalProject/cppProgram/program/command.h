#ifndef COMMAND_H
#define COMMAND_H

#include <cstdint>


//#include "mqtt.h"
#include "OpenInterfaceConfig.h"
#include "../lib/SerialLink/SerialLink.h"

class mqtt;

/**
 *  \class Command defines all actions available for the Roomba and the current state of roomba
 **/
class command
{
	public:
        command() = delete;
		command(mqtt *mq_);
		~command();

		const static void run(int16_t speed);
		static void square(int16_t length);
        const static void sensor(int16_t speed);
        void stop();
        const bool getState();
        void changeState();
        
    private:
        static bool running;
        static SerialLink sl;
        mqtt *mq;
};

bool command::running = false;
SerialLink command::sl = {"/dev/ttyUSB0", static_cast<unsigned int>(Baud::ROOMBA_DEFAULT)};

#endif
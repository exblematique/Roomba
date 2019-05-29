#ifndef COMMAND_H
#define COMMAND_H

#include <cstdint>
#include "lib/SerialLink/SerialLink.h"

class command
{
	public:
		command();
		~command();

		const void run(int16_t speed);
		void square(int16_t length);
        const void sensor(int16_t speed);
        void stop();
        const bool getState();
        void changeState();
        
    private:
        bool running;
        SerialLink sl;
};





            

#endif

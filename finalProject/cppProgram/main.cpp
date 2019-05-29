#include "mqtt.h"

int main(int argc, char *argv[])
{
	class mqtt *clientMqtt;

	mosqpp::lib_init();
    
    if (argc == 2) clientMqtt = new mqtt("RPi", argv[1], 1883);
    if (argc == 3) clientMqtt = new mqtt("RPi", argv[1], atoi(argv[2]));
	else clientMqtt = new mqtt("RPi", "localhost", 1883);
    
	clientMqtt->loop_forever();

	mosqpp::lib_cleanup();

	return 0;
}


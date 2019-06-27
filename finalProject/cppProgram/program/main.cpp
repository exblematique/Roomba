#include "mqtt.h"

int main(int argc, char *argv[])
{
	class mqtt *clientMqtt;

	mosqpp::lib_init();
    
	if (argc == 2){
	  printf("Try to connect in %s on 1883!\n", argv[1]);
	  clientMqtt = new mqtt("RPi", argv[1], 1883);
	}
	else if (argc == 3){
	  printf("Try to connect in %s on %d !\n", argv[1], atoi(argv[2]));
	  clientMqtt = new mqtt("RPi", argv[1], atoi(argv[2]));
	}
	else {
	  printf("Try to connect in localhost !\n");
	  clientMqtt = new mqtt("RPi", "localhost", 1883);
	}
	clientMqtt->loop_forever();

	mosqpp::lib_cleanup();

	return 0;
}


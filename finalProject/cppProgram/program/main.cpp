#ifndef MAIN
#define MAIN

#include "mqtt.h"

/**
 *  \brief This is the main function. It's created a mqtt object and start the receipt. Arguments are not necessarly
 *  \param argv[1] defines the IP address of broker. localhost by default
 *  \param argv[2] defines the listening port of broker. 1883 by default
 *  \return status of program
 **/

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
	clientMqtt->loop_start();

    while (true) ;
    
	mosqpp::lib_cleanup();

	return 0;
}

#endif
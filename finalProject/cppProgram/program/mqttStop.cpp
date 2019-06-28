#include <cstdio>
#include <cstring>
#include <thread>

#include <mosquittopp.h>
#include "mqttStop.h"

/**
 *  \brief mqttStop:mqttStop defines topics and connect to the broker
 *  \param const char *id is a unique identifier to recognize which device has send a message
 *  \param const char *host is the IP address of broker
 *  \param int port is the listening port of broker
 **/
mqttStop::mqttStop(const char *id, const char *host, int port)
    : mosquittopp(id)
    , pubTopic{"roomba/toApi/"}
{
	int keepalive = 60;
	connect(host, port, keepalive);
}

/**
 *  \brief mqtt:mqtt~ delete the current object
 *  \note This function is not used
 **/
mqttStop::~mqttStop()
{
}

/**
 *  \brief mqttStop::on_connect try to connect on broker. 
 *  \param int rc is the error code of connection
 *  \note The program is stopped until the connection is successful
 **/
void mqttStop::on_connect(int rc)
{
	printf("Connected with code %d.\n", rc);
}


/**
 *  \brief mqttStop::sendStop sends to web API the stop signal
 **/
const void mqttStop::sendStop()
{
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Stop system!");
    publish(NULL, (pubTopic + "stop").c_str(), strlen(buf), buf);
}

/**
 *  \brief mqttStop::sendWall sends 'W' to web API on sensor topic
 **/
const void mqttStop::sendWall()
{
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "W");
    publish(NULL, (pubTopic + "sensor").c_str(), strlen(buf), buf);
}

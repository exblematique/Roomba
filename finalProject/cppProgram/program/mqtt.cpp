#include <cstdio>
#include <cstring>
#include <thread>

#include <mosquittopp.h>
#include "mqtt.h"
#include "../lib/SerialLink/SerialLink.h"

/**
 *  \brief mqtt:mqtt defines topics and connect to the broker
 *  \param const char *id is a unique identifier to recognize which device has send a message
 *  \param const char *host is the IP address of broker
 *  \param int port is the listening port of broker
 **/
mqtt::mqtt(const char *id, const char *host, int port)
    : mosquittopp(id)
    , cd{}
    , rootTopic{"roomba/"}
    , pubTopic{(rootTopic + "toApi/").c_str()}
{
	int keepalive = 60;
	connect(host, port, keepalive);
}

/**
 *  \brief mqtt:mqtt~ delete the current object
 *  \note This function is not used
 **/
mqtt::~mqtt()
{
}

/**
 *  \brief mqtt::on_connect try to connect on broker. When in successful, it subscribe on root topic.
 *  \param int rc is the error code of connection
 *  \note The program is stopped until the connection is successful
 **/
void mqtt::on_connect(int rc)
{
	printf("Connected with code %d.\n", rc);
	if(rc == 0){
		/* Only attempt to subscribe on a successful connect. */
		subscribe(NULL, (rootTopic + "#").c_str());
	}
}

/**
 *  \brief mqtt::on_message is called when a new message incoming on the subscribe topic. According to the topic, it's excute the corresponding function on the new thread. 
 *  \param mosquitto_message *message is the incoming message
 *  \note The program is stopped until the connection is successful
 **/
void mqtt::on_message(const struct mosquitto_message *message)
{
    //double temp_celsius, temp_farenheit;
    printf("Message incoming on topic %s: %s\n", message->topic, (char*) message->payload);
    int value;
  
    if (cd.getState()){   //Chech if one program currently running
        if(!strcmp(message->topic, (rootTopic + "stop").c_str())){
            cd.stop();
        }
    } else {
        if(!strcmp(message->topic, (rootTopic + "run").c_str())){
            memset(buf, 0, 51*sizeof(char));	  
            snprintf(buf, 50, "Start system");
            std::thread th{&command::run, 200};
            th.detach();
            publish(NULL, (pubTopic + "run").c_str(), strlen(buf), buf);
        } 
        if(!strcmp(message->topic, (rootTopic + "square").c_str())){
            memset(buf, 0, 51*sizeof(char));	  
            snprintf(buf, 50, "Drawing square");
            memcpy(buf, message->payload, 50*sizeof(char));
            value = atoi(buf);
            std::thread th{&command::square, value};
            th.detach();
            publish(NULL, (pubTopic + "square").c_str(), strlen(buf), buf);
        }
        if(!strcmp(message->topic, (rootTopic + "sensor").c_str())){
            memset(buf, 0, 51*sizeof(char));	  
            snprintf(buf, 50, "Start system with sensor");
            std::thread th{&command::sensor, 200};
            th.detach();
            publish(NULL, (pubTopic + "sensor").c_str(), strlen(buf), buf);
        }
    }
}

/**
 *  \brief mqtt::on_subscribe is called when a subscription is successful
 **/
void mqtt::on_subscribe(int mid, int qos_count, const int *granted_qos)
{
	printf("Subscription succeeded.\n");
}

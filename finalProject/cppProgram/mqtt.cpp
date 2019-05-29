#include <cstdio>
#include <cstring>

#include <mosquittopp.h>
#include "mqtt.h"
#include "lib/SerialLink/SerialLink.h"


mqtt::mqtt(const char *id, const char *host, int port)
    : mosquittopp(id)
    , cd{}
    , rootTopic{"roomba/"}
    , pubTopic{rootTopic + "toApi/"}
{
	int keepalive = 60;

	/* Connect immediately. This could also be done by calling
	 * mqtt->connect(). */
	connect(host, port, keepalive);
}

mqtt::~mqtt()
{
}

void mqtt::on_connect(int rc)
{
	printf("Connected with code %d.\n", rc);
	if(rc == 0){
		/* Only attempt to subscribe on a successful connect. */
		subscribe(NULL, (rootTopic + "#").c_str());
	}
}

void mqtt::on_message(const struct mosquitto_message *message)
{
  //double temp_celsius, temp_farenheit;
  printf("Message incoming on topic %s: %s", message->topic, (char*) message->payload);
  char buf[51];
  int value;
  if(!strcmp(message->topic, (rootTopic + "run").c_str())){
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Start system");
    publish(NULL, (pubTopic + "run").c_str(), strlen(buf), buf);
    cd.run(200);
    /*
    memcpy(buf, message->payload, 50*sizeof(char));
    value = atoi(buf);
    cd.run(value);
    */
  } 
  if(!strcmp(message->topic, (rootTopic + "square").c_str())){
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Drawing square");
    publish(NULL, (pubTopic + "square").c_str(), strlen(buf), buf);
    
    memcpy(buf, message->payload, 50*sizeof(char));
    value = atoi(buf);
    cd.square(value);
  } 

  if(!strcmp(message->topic, (rootTopic + "sensor").c_str())){
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Start system with sensor");
    publish(NULL, (pubTopic + "sensor").c_str(), strlen(buf), buf);
    cd.sensor(200);
    
    /*memcpy(buf, message->payload, 50*sizeof(char));
    value = atoi(buf);
    cd.sensor(value);
    */
  }
  if(!strcmp(message->topic, (rootTopic + "stop").c_str())){
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Stop system");
    publish(NULL, (pubTopic + "stop").c_str(), strlen(buf), buf);
    cd.stop();
  }
  
  
  
  
  
  /*
  if(!strcmp(message->topic, "roomba/start")){
    memset(buf, 0, 51*sizeof(char));	  
    snprintf(buf, 50, "Start system");
    publish(NULL, "roomba/command", strlen(buf), buf);
  } else if(!strcmp(message->topic, "roomba/run")){
    memset(buf, 0, 51*sizeof(char));
    memcpy(buf, message->payload, 50*sizeof(char));
    distance = atoi(buf);
    snprintf(buf, 50, "Romba will be run during %d mm", distance);
    publish(NULL, "roomba/command", strlen(buf), buf);
  } else if(!strcmp(message->topic, "roomba/stop")){
    memset(buf, 0, 51*sizeof(char));
    snprintf(buf, 50, "Romba is stopped");
    publish(NULL, "roomba/command", strlen(buf), buf);
  }*/
}

void mqtt::on_subscribe(int mid, int qos_count, const int *granted_qos)
{
	printf("Subscription succeeded.\n");
}


console.log("Hello world")
ws = new WebSocket("ws://127.0.0.1:4080/rao");

ws.onopen = function(){ 
	console.log("Connection opened");
	ws.send("BALUS");
	ws.send("BALUSST");
	ws.send("WRONG");
	};
ws.onclose = function(event){ console.log("Connection close. Reason: " + event.reason + ":" + event.code); };
ws.onmessage = function(event){ console.log("Got data: " + event.data); };
ws.onerror = function(error){ console.log("Got error: " + error.message); };


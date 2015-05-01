
console.log("Hello world")
ws = new WebSocket("ws://127.0.0.1:4080/rao");

ws.onopen = function(){ 
	console.log("Connection opened");
	ws.send("BALUS");
	ws.send("BALUSST");
	ws.send("WRONG");
	};
ws.onclose = function(){ console.log("Connection close"); };
ws.onmessage = function(event){ console.log("Got data: " + event.data); };
ws.onerror = function(error){ console.log("Got error: " + error.message); };


;(function(){

	function log(text){
		new_text = text + "\n" + $("#logger").text();
		$("#logger").text(new_text);
	}


	// CONNECTIONS SETTINGS
	// ====================
	URL = "ws://127.0.0.1:4080/rao"
	ws = new WebSocket(URL);
	ws.onopen = function(){ 
		log("Connection opened to "+ URL);
		};
	ws.onclose = function(event){ log("Connection close. Reason: " + event.reason + ":" + event.code); };
	ws.onmessage = function(event){ log("Got data: " + event.data); };
	ws.onerror = function(error){ log("Got error: " + error.message); };

	// BUTTONS FUNCTIONS
	// =================
	document.getElementById('connect').addEventListener('click', function(){ log("connect") }, false);
	document.getElementById('connect_room').addEventListener('click', function(){ log("connect room") }, false);
	document.getElementById('disconnect').addEventListener('click', function(){ log("disconnect") }, false);
	document.getElementById('status').addEventListener('click', function(){ log("status") }, false);
})();


;(function(){

	function log(text){
		new_text = text + "\n" + $("#logger").text();
		$("#logger").text(new_text);
	}

	// REQUESTS BUILDER
	// ================
	function getRequestForm(){
		return {"type": "", "api": "1.0", "data": {}}
	}

	function getDisconnect(){
		a = getRequestForm();
		a["type"] = "disconnect";
		return a
	}

	function getConnect(){
		a = getRequestForm();
		a["type"] = "connect";
		data = {"login": $("#login").val(), "password": $("#password").val()};
		a["data"] = data;
		return a;
	}

	function getConnectRoom(){
		a = getRequestForm();
		a["type"] = "connect_room";
		data = {"index": $("#index").val()};
		a["data"] = data;
		return a;
	}

	function getRequest(request_list){
		a = getRequestForm();
		a["type"] = "request";
		a["data"] = {"requests": request_list}
		return a;
	}




	// CONNECTIONS SETTINGS
	// ====================
	URL = "ws://127.0.0.1:4080/rao"
	ws = new WebSocket(URL);
	ws.onopen = function(){ 
		log("Connection opened to "+ URL);
		};
	ws.onclose = function(event){ log("Connection close. Reason: " + event.reason + ":" + event.code); };
	ws.onmessage = function(event){ log("Response: " + event.data); };
	ws.onerror = function(error){ log("Got error: " + error.message); };

	function send(json){
		text = JSON.stringify(json)
		log("Send: " + text)
		ws.send(text);
	}

	// BUTTONS FUNCTIONS
	// =================
	document.getElementById('connect').addEventListener('click', function(){ send(getConnect()) }, false);
	document.getElementById('connect_room').addEventListener('click', function(){ send(getConnectRoom()) }, false);
	document.getElementById('disconnect').addEventListener('click', function(){ send(getDisconnect()); }, false);
	document.getElementById('request').addEventListener('click', function(){ send(getRequest(["roomlist"])); }, false);
	document.getElementById('status').addEventListener('click', function(){ log("status") }, false);
})();


;(function(){

	function log(text){
		new_text = text + "\n" + $("#logger").text();
		$("#logger").text(new_text);
	}

	// REQUESTS BUILDER
	// ================
	function getRequestForm(){
		return {"type": "", "api": "1.0", "data": {}};
	}

	function getDisconnect(){
		a = getRequestForm();
		a["type"] = "disconnect";
		return a;
	}

	function getConnect(){
		a = getRequestForm();
		a["type"] = "connect";
		data = {"login": $("#login").val(), "password": $("#password").val()};
		a["data"] = data;
		return a;
	}

	function getConnectRoom(){
		$("#turn_counter").text(0);
		a = getRequestForm();
		a["type"] = "connect_room";
		data = {"index": $("#index").val()};
		a["data"] = data;
		return a;
	}

	function getRequest(){
		request_list = $("#requests").val().split(" ");
		a = getRequestForm();
		a["type"] = "request";
		a["data"] = {"requests": request_list}
		return a;
	}

	function getStatus(){
		request_list = $("#requests").val().split(" ");
		a = getRequestForm();
		a["type"] = "status";
		a["data"] = {"requests": request_list}
		return a;
	}

	function getControl(action){
		a = getRequestForm();
		a["type"] = "control";
		a["data"] = {"action": action};
		return a;
	}

	function getChat(){
		a = getRequestForm();
		a["type"] = "chat";

		msg = $("#chat_text").val();
		$("#chat_text").val("");
		a["data"] = {"message": msg};
		return a;
	}


	// CONNECTIONS SETTINGS
	// ====================
	URL = "ws://localhost:4080/rao"
	ws = new WebSocket(URL);
	ws.onopen = function(){ 
		log("Connection opened to "+ URL);
		};
	ws.onclose = function(event){ log("Connection close. Reason: " + event.reason + ":" + event.code); };
	ws.onmessage = function(event){ handleMessage(event.data); };
	ws.onerror = function(error){ log("Got error: " + error.message); };

	function send(json){
		text = JSON.stringify(json);
		// log("Send: " + text);
		ws.send(text);
	}

	// BUTTONS FUNCTIONS
	// =================
	document.getElementById('status').addEventListener('click', function(){ send(getStatus()) }, false);
	document.getElementById('connect').addEventListener('click', function(){ send(getConnect()) }, false);
	document.getElementById('request').addEventListener('click', function(){ send(getRequest()); }, false);
	document.getElementById('chat_button').addEventListener('click', function(){ send(getChat()) }, false);
	document.getElementById('disconnect').addEventListener('click', function(){ send(getDisconnect()); }, false);
	document.getElementById('connect_room').addEventListener('click', function(){ send(getConnectRoom()) }, false);


	document.getElementById('control_up').addEventListener('click', function(){ send(getControl(this.id)) }, false);
	document.getElementById('control_down').addEventListener('click', function(){ send(getControl(this.id)) }, false);
	document.getElementById('control_left').addEventListener('click', function(){ send(getControl(this.id)) }, false);
	document.getElementById('control_right').addEventListener('click', function(){ send(getControl(this.id)) }, false);
	document.getElementById('control_action').addEventListener('click', function(){ send(getControl(this.id)) }, false);


	// RENDER FUNCTIONS
	// ================
	var canvas = document.getElementById("myCanvas").getContext("2d");
	var width = 16;

	var img_wall = new Image();
	img_wall.src = "images/wall.png"

	var img_floor = new Image();
	img_floor.src = "images/floor.png"

	var img_player = new Image();
	img_player.src = "images/player.png"

	chat = []
	function handleMessage(data){
		json = JSON.parse(data);
		log(data);
		if (json["code"] != 200 || json["type"] == "status") {
			log(data);
		}
		if (json["type"] == "information") {
			jsonData = JSON.parse(json["data"].replace(/'/g, '"'));

			if (jsonData["tick"]){
				el = $("#turn_counter");
				el.text( parseInt(el.text()) + 1);
			}

			if (jsonData["chat"]){
				msg = jsonData["chat"];
				chat.splice(0, 0, msg);
				chat = chat.slice(0, 20);
				$("#chat_box").text(chat.join("\n"));
			}

			if (jsonData["map"]){
				map = jsonData["map"].split('\n');
				for (i in map){
					map_row = map[i];
					for (j in map_row){
						if (map_row[j] == "#"){
							canvas.drawImage(img_wall, j*width, i*width);
						}
						else if (map_row[j] == "."){
							canvas.drawImage(img_floor, j*width, i*width);
						}
					}
				}
			}

			if (jsonData["players"]){
				players = jsonData["players"];
				for (player in players){
					hero = players[player]["Hero"];
					canvas.drawImage(img_player, hero["X"] * width, hero["Y"] * width);
				}
			};
		}
	}

	// UPDATE FUNCTIONS
	// ================
	// setInterval(function(){ send(getRequest(["map", "players"])); console.log(ws.readyState) }, 333);

	$('body').on('keydown',function(e){
		if (e.keyCode == 87) { send(getControl("control_up")) };
		if (e.keyCode == 83) { send(getControl("control_down")) };
		if (e.keyCode == 65) { send(getControl("control_left")) };
		if (e.keyCode == 68) { send(getControl("control_right")) };
		if (e.keyCode == 32) { send(getControl("control_action")) };
	});

})();


;(function(){

	// UTILS FUNCTIONS
	// ===============

	function arrayUnique(array) {
		var a = this.concat();
		for(var i=0; i<a.length; ++i) {
			for(var j=i+1; j<a.length; ++j) {
				if(a[i] === a[j])
					a.splice(j--, 1);
			}
		}
		return a;
	};

	function log(text){
		new_text = text + "\n" + $("#logger").text();
		$("#logger").text(new_text);
	}

	function extendJson(outputJson, json){
		for (var key in json){
			outputJson[key] = json[key];
		}
		return outputJson;
	}

	function extendMap(outputMap, map){
		for (var key in map){
			if (!outputMap.hasOwnProperty(key)){
				outputMap[key] = [];
			}
			// outputMap[key] = _.union(outputMap[key], map[key])
			for (var i in map[key]){
				var exist = false;
				for (var j in outputMap[key]){
					if (map[key][i][0] === outputMap[key][j][0] && map[key][i][1] === outputMap[key][j][1]){
						exist = true;
						break;
					}
				}
				if (!exist){
					outputMap[key].push(map[key][i])
				}
			}
		}
		return outputMap;
	};


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
	URL = "ws://127.0.0.1:4080/rao"
	// URL = "ws://45.55.134.136:4080/rao"
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
	var canvas_el = document.getElementById("myCanvas")
	var canvas = canvas_el.getContext("2d");
	canvas.font = "14px Arial";
	var width = 32;

	var Images = {
		"Wall": loadImage("Wall"), 
		"Floor": loadImage("Floor")
	};

	function loadImage(name){
		src = "images/" + name + ".png";
		img = new Image();
		img.src = src;
		console.log("Loading image: " + src);
		return img;
	}

	game = {"map": {}, "players": "", "player": "", "entities": ""};
	myPlayer = {}

	chat = []
	function handleMessage(data){
		json = JSON.parse(data);
		jsonData = JSON.parse(json["data"].replace(/'/g, '"'));
		log(data);
		if (json["code"] != 200 || json["type"] == "status") {
			log(data);
		}
		if (json["type"] == "status") {
			if (jsonData["player"]){
				myPlayer = jsonData["player"];
			}
		}
		if (json["type"] == "information") {
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
				game["map"] = extendMap(game["map"], jsonData["map"])
				game["activeMap"] = jsonData["map"];
			}

			if (jsonData["players"]){
				players = jsonData["players"];
				game["players"] = players;

				for (player in players){
					if (players[player]["Name"] == myPlayer["Name"]){
						myPlayer = players[player]
					}
				}
			}

			if (jsonData["entities"]){
				game["entities"] = jsonData["entities"];
				console.log(game["entities"])
			}
		}
	}

	// var d = new Date();
	function draw(){
		var curTime = new Date().getTime();
		canvas.clearRect(0, 0, canvas_el.width, canvas_el.height);
		if (myPlayer["Hero"]){
			var offsetX = (myPlayer["Hero"]["X"] * width) - canvas_el.width/2;
			var offsetY = (myPlayer["Hero"]["Y"] * width) - canvas_el.height/2;
		}

		// In-Game loading images:
		for (i in game["entities"]){
			img = game["entities"][i]["Image"];
			if (!Images.hasOwnProperty(img)){
				Images[img] = loadImage(img);
			}
		}
		// Draw all map
		for (i in game["map"]){
			tileList = game["map"][i];

			for (pos in tileList) {
				var x = tileList[pos][0];
				var y = tileList[pos][1];
				var drawX = x*width - offsetX;
				var drawY = y*width - offsetY;

				// if this tile on screen:
				if (drawX > -width && drawX < canvas_el.width && drawY >= -width && drawY < canvas_el.height){
					if (i == "#"){
						canvas.drawImage(Images["Wall"], drawX, drawY);
					}
					else if (i == "."){
						canvas.drawImage(Images["Floor"], drawX, drawY);
					}
					canvas.globalAlpha = 0.2;
					canvas.fillStyle = "#000000";
					canvas.fillRect(x*width - offsetX, y*width - offsetY, width, width);
					canvas.globalAlpha = 1;
				}
			}
		}

		// Draw active map
		for (i in game["activeMap"]){
			tileList = game["activeMap"][i];

			for (pos in tileList) {
				var x = tileList[pos][0];
				var y = tileList[pos][1];
				if (i == "#"){
					canvas.drawImage(Images["Wall"], x*width - offsetX, y*width - offsetY);
				}
				else if (i == "."){
					canvas.drawImage(Images["Floor"], x*width - offsetX, y*width - offsetY);
				}
			}
		}

		// console.log(game);
		// Draw entities
		for (i in game["entities"]){
			entity = game["entities"][i];

			img = entity["Image"];
			canvas.drawImage(Images[img], entity["X"] * width - offsetX, entity["Y"] * width - offsetY);

		}

		// HP bar:
		for (i in game["entities"]){
			entity = game["entities"][i];
			if (entity["Type"] == "Enemy" || entity["Type"] == "Player"){
				// render simple GUI
				bar_width = 32;
				bar_height = 8;
				c_width = canvas_el.width
				var ent_x = entity["X"] * width - offsetX
				var ent_y = entity["Y"] * width - offsetY;

				canvas.fillStyle = "#000000";
				canvas.fillRect(ent_x, ent_y+32, bar_width, bar_height);
				canvas.fillStyle = "#FF0000";
				canvas.fillRect(ent_x, ent_y+32, bar_width * (entity["Health"]/entity["HealthMax"]), bar_height);
				// canvas.fillStyle = "#000000";
				// canvas.fillText(entity["Name"], ent_x, ent_y+38);
			}
		}
		// console.log(new Date().getTime() - curTime)
	}

	// UPDATE FUNCTIONS
	// ================
	setInterval(function(){ draw(); }, 25);

	$('body').on('keydown',function(e){
		if (e.keyCode == 87) { send(getControl("control_up")) };
		if (e.keyCode == 83) { send(getControl("control_down")) };
		if (e.keyCode == 65) { send(getControl("control_left")) };
		if (e.keyCode == 68) { send(getControl("control_right")) };
		if (e.keyCode == 32) { send(getControl("control_action")) };
	});
})();
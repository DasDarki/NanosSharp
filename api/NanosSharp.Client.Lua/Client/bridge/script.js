const url = "ws://localhost:25302/.nanos-sharp/bridge";
let ws;

function open() {
    ws = new WebSocket(url);
    ws.onmessage = function (e) {
        handleMessage(e.data);
    };
    ws.onclose = function() {
        open();
    };
}

function handleMessage(msg) {
    Events.Call("nanos-bridge:call", msg);
}

function canSend() {
    return !!ws && ws.readyState === ws.OPEN;
}

function sendMessage(msg) {
    if (canSend()) {
        ws.send(msg);
    } else {
        const interval = setInterval(() => {
            if (canSend()) {
                ws.send(msg);
                clearInterval(interval);
            }
        }, 100);
    }
}

Events.Subscribe("nanos-bridge:call-event", function (event, ...args) {
    sendMessage(JSON.stringify({ event, args }));
});

Events.Subscribe("nanos-bridge:call-rpc", function (callID, returnValue) {
    sendMessage(JSON.stringify({ callID, result: returnValue }));
});

open()
"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.start().then(function () {
    console.log("connected to hub");
    // Automatically start a chat when connected
    connection.invoke("StartChat").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (message) {
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message", "received");

    const msgContent = document.createElement("div");
    msgContent.classList.add("message-content");
    msgContent.textContent = message;

    msgDiv.appendChild(msgContent);
    document.getElementById("messagesList").appendChild(msgDiv);
});


connection.on("ConnectedToPartner", function (partnerId) {
    console.log("Connected to partner: " + partnerId);
});

connection.on("PartnerDisconnected", function () {
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message", "received");

    const msgContent = document.createElement("div");
    msgContent.classList.add("message-content");
    msgContent.textContent = "Your partner has disconnected.";

    msgDiv.appendChild(msgContent);
    document.getElementById("messagesList").appendChild(msgDiv);
});

connection.on("NoAvailablePartners", function () {
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message", "received");

    const msgContent = document.createElement("div");
    msgContent.classList.add("message-content");
    msgContent.textContent = "No partners available at the moment.";

    msgDiv.appendChild(msgContent);
    document.getElementById("messagesList").appendChild(msgDiv);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = document.getElementById("messageInput").value;
    if (message.trim() !== "") {
        const msgDiv = document.createElement("div");
        msgDiv.classList.add("message", "sent");

        const msgContent = document.createElement("div");
        msgContent.classList.add("message-content");
        msgContent.textContent = message;

        msgDiv.appendChild(msgContent);
        document.getElementById("messagesList").appendChild(msgDiv);

        connection.invoke("SendMessage", message).catch(function (err) {
            return console.error(err.toString());
        });

        document.getElementById("messageInput").value = "";
    }
    event.preventDefault();
});

document.getElementById("endChatButton").addEventListener("click", function (event) {
    connection.invoke("EndChat").catch(function (err) {
        return console.error(err.toString());
    });

    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message", "received");

    const msgContent = document.createElement("div");
    msgContent.classList.add("message-content");
    msgContent.textContent = "You have ended the chat.";

    msgDiv.appendChild(msgContent);
    document.getElementById("messagesList").appendChild(msgDiv);

    event.preventDefault();
});

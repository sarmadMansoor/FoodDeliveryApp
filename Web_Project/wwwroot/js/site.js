"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

/*connection.on("ReceiveCartUpdate", function (message) {
    *//*Toastify({
        text: message,   // Message received from SignalR
        duration: 3000,  // Display time (3s)
        gravity: "top",  // Position (top, bottom)
        position: "right", // Left, Center, or Right
        backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)", // Custom style
        stopOnFocus: true, // Stop timer on hover
    }).showToast();*//*
    console.alert("Bello");
});*/

/*connection.on("ReceiveCartUpdate", function (message) {
    // Show the alert
    var alertBox = document.getElementById("cart-alert");
    alertBox.innerText = message; // Set message from SignalR
    alertBox.style.display = "block";

    // Hide the alert after 3 seconds
    setTimeout(function () {
        alertBox.style.display = "none";
    }, 3000);
});*/


connection.on("ReceiveCartUpdate", function (username, productName) {
    // Show the pop-up with user and product name
    var popup = document.getElementById("cart-popup");
    var message = document.getElementById("popup-message");

    message.innerText = username + " added " + productName + " to the cart!";
    popup.style.display = "block";

    // Hide the pop-up after 4 seconds
    setTimeout(function () {
        popup.style.display = "none";
    }, 4000);
});


connection.start().then(function () {
    console.log("Connection started");
}).catch(function (err) {
    console.error("Connection failed:", err.toString());
});

document.getElementById("sendButtonCart").addEventListener("click", function (event) {
    console.log("hello");

    connection.invoke("NotifyCartUpdate", "Hello").catch(function (err) {
        console.error("Failed to send message:", err.toString());
    });
    event.preventDefault();
})
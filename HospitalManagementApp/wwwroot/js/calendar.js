var staffData = document.getElementById("cal").innerText;

var a2 = document.querySelectorAll('.a2');
var a3 = document.querySelectorAll('.a3');
var a4 = document.querySelectorAll('.a4');
var a5 = document.querySelectorAll('.a5');
var a6 = document.querySelectorAll('.a6');
var a7 = document.querySelectorAll('.a7');
var a8 = document.querySelectorAll('.a8');


var b2 = document.querySelectorAll('.b2');
var b3 = document.querySelectorAll('.b3');
var b4 = document.querySelectorAll('.b4');
var b5 = document.querySelectorAll('.b5');
var b6 = document.querySelectorAll('.b6');
var b7 = document.querySelectorAll('.b7');
var b8 = document.querySelectorAll('.b8');

//xu li
var lines = staffData.split(' ');
for (var i = 0; i < lines.length; i+=2) {
    var p1 = lines[i];
    var p2 = lines[i + 1];

    switch (p1) {
        case "a2":
            a2.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b2":
            b2.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a3":
            a3.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b3":
            b3.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a4":
            a4.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b4":
            b4.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a5":
            a5.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b5":
            b5.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a6":
            a6.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b6":
            b6.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a7":
            a7.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b7":
            b7.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "a8":
            a8.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        case "b8":
            b8.forEach(function (cell) {
                cell.innerText = p2;
            });
            break;
        default:
            break;
    }
}
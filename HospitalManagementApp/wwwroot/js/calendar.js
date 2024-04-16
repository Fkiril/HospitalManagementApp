var staffData = document.getElementById("cal").innerText;

const a2 = document.getElementById('a2');
const a3 = document.getElementById('a3');
const a4 = document.getElementById('a4');
const a5 = document.getElementById('a5');
const a6 = document.getElementById('a6');
const a7 = document.getElementById('a7');
const a8 = document.getElementById('a8');


const b2 = document.getElementById('b2');
const b3 = document.getElementById('b3');
const b4 = document.getElementById('b4');
const b5 = document.getElementById('b5');
const b6 = document.getElementById('b6');
const b7 = document.getElementById('b7');
const b8 = document.getElementById('b8');

const c2 = document.getElementById('c2');
const c3 = document.getElementById('c3');
const c4 = document.getElementById('c4');
const c5 = document.getElementById('c5');
const c6 = document.getElementById('c6');
const c7 = document.getElementById('c7');
const c8 = document.getElementById('c8');

function morning(element1, element2, element3) {
    element1.innerText = "Work";
    element2.innerText = "";
    element3.innerText = "";
}
function afternoon(element1, element2, element3) {
    element1.innerText = "Work";
    element2.innerText = "";
    element3.innerText = "";
}
function evening(element1, element2, element3) {
    element1.innerText = "Work";
    element2.innerText = "";
    element3.innerText = "";
}

function day(text, element1, element2, element3) {
    if (text == "morning") morning(element1, element2, element3);
    else if (text == "afternoon") afternoon(element1, element2, element3);
    else evening(element1, element2, element3);
}

//xu li
var lines = staffData.split(' ');
for (var i = 0; i < lines.length; i+=2) {
    var p1 = lines[i];
    var p2 = lines[i + 1];

    switch (p1) {
        case "Monday":
            day(p2, a2, b2, c2);
            break;
        case "Tuesday":
            day(p2, a3, b3, c3);
            break;
        case "Wednesday":
            day(p2, a4, b4, c4);
            break;
        case "Thursday":
            day(p2, a5, b5, c5);
            break;
        case "Friday":
            day(p2, a6, b6, c6);
            break;
        case "Saturday":
            day(p2, a7, b7, c7);
            break;
        case "Sunday":
            day(p2, a8, b8, c8);
            break;
        default:
            break;
    }
}
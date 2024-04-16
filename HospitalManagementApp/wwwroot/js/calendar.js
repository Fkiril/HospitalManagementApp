var date = document.getElementById("date").innerText;
var shift = document.getElementById("shift").innerText;

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

const day1 = document.getElementById('day1');
const day2 = document.getElementById('day2');
const day3 = document.getElementById('day3');
const day4 = document.getElementById('day4');
const day5 = document.getElementById('day5');
const day6 = document.getElementById('day6');
const day7 = document.getElementById('day7');

function morning(element1, element2, element3) {
    element1.innerText = "Work";
    element2.innerText = "";
    element3.innerText = "";
}
function afternoon(element1, element2, element3) {
    element1.innerText = "";
    element2.innerText = "Work";
    element3.innerText = "";
}
function evening(element1, element2, element3) {
    element1.innerText = "";
    element2.innerText = "";
    element3.innerText = "Work";
}

function day(text, element1, element2, element3) {
    if (text == "Morning") morning(element1, element2, element3);
    else if (text == "Afternoon") afternoon(element1, element2, element3);
    else evening(element1, element2, element3);
}

//xu li
var lines = shift.split(", ");

//update for calendar
day(lines[0], a2, b2, c2);
day(lines[1], a3, b3, c3);
day(lines[2], a4, b4, c4);
day(lines[3], a5, b5, c5);
day(lines[4], a6, b6, c6);
day(lines[5], a7, b7, c7);
day(lines[6], a8, b8, c8);


var dates = date.split(", ");

day1.innerText = dates[0];
day2.innerText = dates[1];
day3.innerText = dates[2];
day4.innerText = dates[3];
day5.innerText = dates[4];
day6.innerText = dates[5];
day7.innerText = dates[6];
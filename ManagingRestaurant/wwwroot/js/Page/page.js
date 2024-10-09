// main
let menuNav = document.querySelector("#menu-nav")
let menuMobile = document.querySelector("#navbar-mobile")


menuNav.onclick = function(){
    menuMobile.classList.toggle("appear")
}


var days = 0;
var hours = 1;
var minutes = 1;
var seconds = 12;

var time = setInterval(() => {
  if (days !== 0) {
    // day != 0 && hours!=0
    if (hours !== 0) {
      // day != 0 && hours!=0 minutes !==0
      if (minutes !== 0) {
        // day != 0 && hours!=0 minutes !==0 seconds !=0
        if (seconds !== 0) {
          seconds--;
        }
        // day != 0 && hours!=0 minutes !==0 seconds =0
        else {
          seconds = 59;
          minutes--;
        }
      }
      // day != 0 && hours!=0 minutes =0
      else {
        // day != 0 && hours!=0 minutes ==0 seconds !=0
        if (seconds !== 0) {
          seconds--;
        }
        // day != 0 && hours!=0 minutes ==0 seconds =0
        else {
          hours--;
          seconds = 59;
          minutes = 59;
        }
      }
    }
    // day != 0 && hours==0
    else {
      // day != 0 && hours==0 minutes !==0
      if (minutes !== 0) {
        if (seconds !== 0) {
          seconds--;
        } else {
          seconds = 59;
          minutes--;
        }
      } else {
        if (seconds === 0) {
          seconds--;
        } else {
          seconds = 59;
          minutes = 59;
          hours = 23;
          days--;
        }
      }
    }
  }
  // days=0
  else {
    if (hours !== 0) {
      if (minutes !== 0) {
        if (seconds !== 0) {
          seconds--;
        } else {
          seconds = 59;
          minutes--;
        }
      } else {
        // days =0 hours !=0 minutes==0
        if (seconds !== 0) {
          seconds--;
        } else {
          seconds = 59;
          minutes = 59;
          hours--;
        }
      }
    }
    // day =0 hour =0
    else {
      if (minutes !== 0) {
        if (seconds !== 0) {
          seconds--;
        } else {
          seconds = 59;
          minutes--;
        }
      } else {
        // days =0 hours ==0 minutes==0
        if (seconds !== 0) {
          seconds--;
        } else {
          clearInterval(time);
          document.querySelector("button").style.backgroundColor = "#fb8f2c";
          document.querySelector("#button").addEventListener("click", () => {
            window.open("shopping.html");
          });
        }
      }
    }
  }
  if (seconds < 10)
    document.querySelector(".seconds").innerHTML = `0${seconds}`;
  else document.querySelector(".seconds").innerHTML = `${seconds}`;

  if (minutes < 10)
    document.querySelector(".minutes").innerHTML = `0${minutes}`;
  else document.querySelector(".minutes").innerHTML = `${minutes}`;

  if (hours < 10) document.querySelector(".hours").innerHTML = `0${hours}`;
  else document.querySelector(".hours").innerHTML = `${hours}`;

  if (days < 10) document.querySelector(".days").innerHTML = `0${days}`;
  else document.querySelector(".days").innerHTML = `${days}`;
}, 1000);

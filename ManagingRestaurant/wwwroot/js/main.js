let menuNav = document.querySelector("#menu-nav")
let menuMobile = document.querySelector("#navbar-mobile")


menuNav.onclick = function(){
    menuMobile.classList.toggle("appear")
}

let menuList = Array.from(document.querySelectorAll(".menu__list__item"))
let menuNavButton = Array.from(document.querySelectorAll(".menu__navbar button"))



function filterProduct(element){
    console.log(element)
    let elementType = element.getAttribute("type").toUpperCase()
    menuNavButton.forEach(menuNavButtonItem=>{
        let typeButton = menuNavButtonItem.getAttribute("type")
        if(typeButton.toUpperCase() == elementType){
            element.classList.add("mainColor")
        }else {
            menuNavButtonItem.classList.remove("mainColor")
        }
    })
    menuList.forEach(item=>{
        console.log(item)
        let menuItemImg = item.querySelector(".list__item-about-img img")
        let categoryImg = menuItemImg.getAttribute("type").toUpperCase();
        if(elementType=="ALL" || categoryImg == elementType){
            item.parentElement.classList.remove("hide")
        }else {
            item.parentElement.classList.add("hide")
        }
    })
}


function developing(){
    alert("Dự án đang phát triển tính năng này")
}
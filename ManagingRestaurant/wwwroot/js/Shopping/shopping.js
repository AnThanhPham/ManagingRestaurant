// header 
let menuNav = document.querySelector("#menu-nav")
let menuMobile = document.querySelector("#navbar-mobile")

menuNav.onclick = function(){
    menuMobile.classList.toggle("appear")
}

//localStorage.clear();
// lọc sản phẩm
let menuListCart = Array.from(document.querySelectorAll(".card"))
let menuNavButtonCart = Array.from(document.querySelectorAll(".control button"))

function filterProduct(element){
    console.log(element)
    let elementType = element.getAttribute("type").toUpperCase()
    menuNavButtonCart.forEach(menuNavButtonItem=>{
        let typeButton = menuNavButtonItem.getAttribute("type")
        if(typeButton.toUpperCase() == elementType){
            element.classList.add("active")
        }else {
            menuNavButtonItem.classList.remove("active")
        }
    })
    menuListCart.forEach(item=>{
        console.log(item)
        let menuItemImg = item.querySelector(".card img")
        let categoryImg = menuItemImg.getAttribute("type").toUpperCase();
        if(elementType=="ALL" || categoryImg == elementType){
            item.parentElement.classList.remove("hide")
        }else {
            item.parentElement.classList.add("hide")
        }
    })
}

// tạo mảng chứa thông tin sản phẩm
const cards = document.querySelectorAll('.card')
const products = []

cards.forEach(card =>{
    const product = {}
    product.name = card.querySelector('.card-title').textContent
    product.price = card.querySelector('.card-text').textContent
    product.id = card.querySelector('.card-img-top').getAttribute('id')
    product.src = card.querySelector('.card-img-top').getAttribute('src')
    products.push(product);
})

console.log(products)

// kiểm tra lưu trữ giỏ hàng - có lấy ra - k trả về rỗng 
let productInCart = localStorage.getItem("products")? JSON.parse(localStorage.getItem("products")) : []

function addToCart(value) {
    var idCard = value
    let checkProduct = productInCart.some(value => value.id === idCard);

    if (!checkProduct) {
        // tìm kiếm - nếu chưa có gán số lần xuất hiện là 1 
      let product = products.find(value => value.id === idCard);
      productInCart.unshift({
        ...product,
        quantity: 1
      });

      // hiện thông báo 
      let message = ``;
      message += `<div class="message success">
        <div class="message__check">
          <i class="fa-sharp fa-solid fa-circle-check fa-2xl" style="color: #73fb18;"></i>
        </div>
        <p class="message__title">The product has been added to cart</p>
        <div class="message__exit">
          <button>
          <i class="fa-solid fa-xmark fa-2xl" style="color: #9ca7ba;"></i>
        </button>
        </div>
      </div>`
    document.querySelector('.Toast_message').innerHTML = message
    } 
    else {
        // tìm kiếm - nếu đã có tăng số lần xuất hiện
      let getIndex = productInCart.findIndex(value => value.id === idCard);
      let product = productInCart.find(value => value.id === idCard);
      productInCart[getIndex] = {
        ...product,
        quantity: ++product.quantity
      };
      
      let message =``
      message +=`<div class="message warning">
        <div class="message__check">
          <i class="fa-solid fa-bug fa-2xl" style="color: #f71d32;"></i>
        </div>
        <p class="message__title">The product already exists int the shopping cart</p>
        <div class="message__exit">
          <button>
          <i class="fa-solid fa-xmark fa-2xl" style="color: #9ca7ba;"></i>
          </button>
        </div>
      </div>  `
    document.querySelector('.Toast_message').innerHTML = message
    }
    localStorage.setItem('products', JSON.stringify(productInCart));
    calculatorTotal()
    messages()
  }

  // xóa thông báo
function messages(){
    let message_btn = document.querySelectorAll('.message__exit button')
    let message = document.querySelectorAll('.message')
    for(let i=0;i<message_btn.length;i++){
        message_btn[i].addEventListener('click',function(){
            message[i].classList.add('hide')
        })
    }
}
// số sản phẩm có trong giỏ hàng 
function calculatorTotal(){
    document.getElementById('quantity').innerHTML=productInCart.length
}
calculatorTotal()

// in ra sản phẩm trong giỏ hàng
function renderProductsCart(){
    let data = ``
    productInCart.map((value,index)=>{
        data += `
        <ul class="cart-ul">
            <li class="col-3 col-sm-3 col-md-3">${value.name}</li>
            <li class="col-2 col-sm-2 col-md-2 text-center">
                <img src='${value.src}' width='100' alt='' >
            </li>
            <li class="col-2 col-sm-2 col-md-2 text-center">$ ${value.price}</li>
            <li class="col-1 col-sm-1 col-md-2 cart-quantity text-center">
                <button onclick ='plusQuantity(${index})' class='btn btn-secondary'>+</button>
                <span class='mx2'>${value.quantity}</span>
                <button onclick ='minusQuantity(${index},${value.quantity})' class='btn btn-secondary'>-</button>
            </li>
            <li class="col-2 col-sm-2 col-md-2 cart-total text-center">$ ${value.quantity * value.price}</li>
            <li class="col-2 col-sm-2 col-md-1">
                <button onclick ='deleteCart(${index})'class="btn btn-danger delete">Delete</button>
            </li>
        </ul>
        `
    })
    document.getElementById('products-cart-body').innerHTML=data
    sumPrice()
}

// lưu vào trình duyệt
function saveToLocalStorage () {
    localStorage.setItem('products', JSON.stringify(productInCart));
  }

// tăng giảm số lượng sản phẩm
function plusQuantity(index){
    // gọi đến giỏ hàng
    productInCart[index] ={
        ...productInCart[index],
        quantity: ++productInCart[index].quantity
    }
    saveToLocalStorage()
    renderProductsCart()
}

function minusQuantity(index,quantity){
    // số lần chọn > 1
    if(quantity > 1){
        productInCart[index] ={
            ...productInCart[index],
            quantity: --productInCart[index].quantity
        }
        saveToLocalStorage()
        renderProductsCart()
    }
    else {alert("Quantity min is 1")}
}

//xóa sản phẩm
function deleteCart(index){
    // xóa theo vị trí và số lượng cần xóa
    productInCart.splice(index,1)
    saveToLocalStorage()
    renderProductsCart()
}

// tính tiền
function sumPrice(){
    if(productInCart.length !== 0){
        let sum=0;
        for(let i=0;i<productInCart.length;i++)
            sum+=productInCart[i].quantity * productInCart[i].price;
        document.getElementById('total-money').innerHTML=sum;
    }
    else document.getElementById('total-money').innerHTML= `0`
}
function cartLoadPage(){
    renderProductsCart();
    calculatorTotal();
}

function cartLoadPage_Cart(){
    renderProductsCart();
}

//search

// tạo hiệu ứng mở ra
var btnSearch = document.querySelector('.search-btn')

btnSearch.addEventListener('click',function(){
    this.parentElement.classList.toggle('open')
    console.log()
})

// hàm tìm kiếm
var searchInput = document.querySelector('#myInput')
searchInput.addEventListener('keyup',function(e){
    // xóa dấu cách thừa từ ô nhập vào
    let txtSearch = e.target.value.trim().toLowerCase()
    let ListProductDOM = document.querySelectorAll('.card')
    // duyệt mảng
    ListProductDOM.forEach(item=>{
        console.log(item)
        if(item.innerText.toLowerCase().includes(txtSearch))
           item.parentElement.classList.remove('hide')
        else item.parentElement.classList.add('hide')
    })
})


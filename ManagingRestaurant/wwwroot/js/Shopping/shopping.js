async function postData(url = "", data = {}) {
    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            // Xử lý lỗi nếu phản hồi không thành công
            throw new Error('Error:', response.status);
        }

        return response.json(); // parses JSON response into native JavaScript objects
    } catch (error) {
        console.error('Fetch error:', error);
        throw error; // Ném lỗi để bắt ở nơi gọi hàm
    }
}
async function getData(url = "") {
    try {
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error('Error:', response.status);
        }

        return await response.json(); // parses JSON response into native JavaScript objects
    } catch (error) {
        console.error('Fetch error:', error);
        throw error; // Ném lỗi để bắt ở nơi gọi hàm
    }
}


async function AddItemToCart() {
    try {
        const data = await getData('/Cart/Home/GetSession');
        let item = document.querySelector('.checkout_items')
        if (item != null) {
            item.textContent = data.TotalQuantity != null ? data.TotalQuantity : 0;
        }
        return data;
    } catch (error) {
        console.error('Fetch error:', error);
    }
}
AddItemToCart();
async function AddToCart() {
    const btnAddToCart = document.querySelectorAll(".add_to_cart_btn");
    btnAddToCart.forEach((btn) => {
        btn.addEventListener("click", async function () {
            const id = btn.getAttribute('data-id');
            const quantity = btn.getAttribute('data-quantity') == null ? 1 : document.getElementById('quantity_value').textContent;
            try {
                const data = await postData('/Cart/Home/AddToCart', { id: id, quantity: quantity });
                if (!data.success) {
                    throw new Error(data.message);
                }
                await AddItemToCart();
                alert(data.message);
            } catch (error) {
                window.location.href = '/login';
            }
        })
    })
}
AddToCart();


async function IncreaseQuantity(that) {
    const id = that.getAttribute('data-id');
    const quantity = 1;
    try {
        const data = await postData('/Cart/Home/AddToCart', { id: id, quantity: quantity });
        if (data.success) {
            const result = await AddItemToCart();
            document.querySelector('.total_price').textContent = (result.TotalPrice).toFixed(2);
            let thisElement = document.getElementById(id);           
            let price = thisElement.querySelector('.item_price');
            let quantity = thisElement.querySelector('.item_quantity');
            let totalPrice = thisElement.querySelector('.item_total_price');
            quantity.innerHTML = parseFloat(quantity.innerHTML) + 1;
            totalPrice.innerHTML = (parseFloat(price.innerHTML) * parseFloat(quantity.innerHTML)).toFixed(2);         
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DecreaseQuantity(that) {
    const id = that.getAttribute('data-id');
    const quantity = 1;
    try {
        const data = await postData('/Cart/Home/DecreaseToCart', { id: id, quantity: quantity });
        if (data.success) {
            const result = await AddItemToCart();
            document.querySelector('.total_price').textContent = result.TotalPrice;
            let thisElement = document.getElementById(id);
            let price = thisElement.querySelector('.item_price');
            let quantity = thisElement.querySelector('.item_quantity');
            let totalPrice = thisElement.querySelector('.item_total_price');
            quantity.innerHTML = parseFloat(quantity.innerHTML) - 1;
            totalPrice.innerHTML = (parseFloat(price.innerHTML) * parseFloat(quantity.innerHTML)).toFixed(2);
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DeleteCartProduct(that) {
    const id = that.getAttribute('data-id');
    try {
        const data = await postData('/Cart/Home/DeleteToCart', { id: id, quantity: 1 });
        if (data.success) {
            await AddItemToCart();
            window.location.reload();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DeleteCartAll() {
    try {
        const data = await postData('/Cart/Home/DeleteToCart', { id: "", quantity: -1 });
        if (data.success) {
            await AddItemToCart();
            window.location.reload();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

initQuantity();

function initQuantity() {
    if ($('.plus').length && $('.minus').length) {
        var plus = $('.plus');
        var minus = $('.minus');
        var value = $('#quantity_value');

        plus.on('click', function () {
            var x = parseInt(value.text());
            value.text(x + 1);
        });

        minus.on('click', function () {
            var x = parseInt(value.text());
            if (x > 1) {
                value.text(x - 1);
            }
        });
    }
}

initThumbnail();

function initThumbnail() {
    if ($('.single_product_thumbnails ul li').length) {
        var thumbs = $('.single_product_thumbnails ul li');
        var singleImage = $('.single_product_image_background');

        thumbs.each(function () {
            var item = $(this);
            item.on('click', function () {
                thumbs.removeClass('active');
                item.addClass('active');
                var img = item.find('img').data('image');
                singleImage.css('background-image', 'url(' + img + ')');
            });
        });
    }
}
/* 

    6. Init Star Rating

    */

function initStarRating() {
    if ($('.user_star_rating li').length) {
        var stars = $('.user_star_rating li');

        stars.each(function () {
            var star = $(this);

            star.on('click', function () {
                var i = star.index();

                stars.find('i').each(function () {
                    $(this).removeClass('fa-star');
                    $(this).addClass('fa-star-o');
                });
                for (var x = 0; x <= i; x++) {
                    $(stars[x]).find('i').removeClass('fa-star-o');
                    $(stars[x]).find('i').addClass('fa-star');
                };
            });
        });
    }
}

initStarRating();
/* 

    8. Init Tabs

    */

function initTabs() {
    if ($('.tabs').length) {
        var tabs = $('.tabs li');
        var tabContainers = $('.tab_container');

        tabs.each(function () {
            var tab = $(this);
            var tab_id = tab.data('active-tab');

            tab.on('click', function () {
                if (!tab.hasClass('active')) {
                    tabs.removeClass('active');
                    tabContainers.removeClass('active');
                    tab.addClass('active');
                    $('#' + tab_id).addClass('active');
                }
            });
        });
    }
}
initTabs();



// Lấy danh sách các sản phẩm và các nút điều khiển lọc
let menuListCart = Array.from(document.querySelectorAll(".card"));
let menuNavButtonCart = Array.from(document.querySelectorAll(".sidebar_categories .control-button a"));

// Hàm lọc sản phẩm
function filterProduct(element) {
    console.log(element);
    let elementType = element.getAttribute("data-filter").toUpperCase();

    // Cập nhật nút active
    menuNavButtonCart.forEach(menuNavButtonItem => {
        if (menuNavButtonItem === element) {
            menuNavButtonItem.parentElement.classList.add("active");
        } else {
            menuNavButtonItem.parentElement.classList.remove("active");
        }
    });

    // Lọc các mục trong danh sách sản phẩm
    menuListCart.forEach(item => {
        let categoryClass = Array.from(item.classList).find(cls => cls.startsWith("category-"));
        if (categoryClass) categoryClass = categoryClass.toUpperCase();

        if (elementType === "*" || categoryClass === elementType.slice(1)) {
            item.parentElement.classList.remove("hide");
        } else {
            item.parentElement.classList.add("hide");
        }
    });
}

// Gán sự kiện onclick cho các nút lọc
menuNavButtonCart.forEach(button => {
    button.addEventListener("click", function (event) {
        event.preventDefault(); // Ngăn chặn điều hướng mặc định của thẻ <a>
        filterProduct(this);
    });
});


// Hiệu ứng mở khi nhấn vào nút tìm kiếm
var btnSearch = document.querySelector('.search-btn');
btnSearch.addEventListener('click', function () {
    this.parentElement.classList.toggle('open');
    console.log('Toggled search bar');
});

// Hàm tìm kiếm sản phẩm
var searchInput = document.querySelector('#myInput');
searchInput.addEventListener('keyup', function (e) {
    // Lấy giá trị nhập vào và xóa dấu cách thừa, chuyển sang chữ thường
    let txtSearch = e.target.value.trim().toLowerCase();
    let ListProductDOM = document.querySelectorAll('.card');

    // Duyệt qua danh sách sản phẩm để lọc theo từ khóa
    ListProductDOM.forEach(item => {
        let productName = item.innerText.toLowerCase();
        let categoryClass = Array.from(item.classList).find(cls => cls.startsWith("category-"));

        // Hiển thị hoặc ẩn sản phẩm dựa trên từ khóa tìm kiếm
        if (productName.includes(txtSearch) || txtSearch === "") {
            item.parentElement.classList.remove('hide');
        } else {
            item.parentElement.classList.add('hide');
        }
    });
});
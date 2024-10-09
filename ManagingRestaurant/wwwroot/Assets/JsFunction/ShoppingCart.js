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
async function AddItemToCart(){
    try {
        const data = await getData('/Cart/Home/GetSession');
        let item = document.querySelector('.checkout_items')
        if(item != null){
            item.textContent = data.TotalQuantity!=null ? data.TotalQuantity : 0;
        }
        return data;
    } catch (error) {
        console.error('Fetch error:', error);
    }
}
AddItemToCart();
async function AddToCart (){
    const btnAddToCart = document.querySelectorAll(".add_to_cart_btn");
    btnAddToCart.forEach((btn)=>{
        btn.addEventListener("click", async function(){
            const id = btn.getAttribute('data-id');
            const quantity = btn.getAttribute('data-quantity')==null ? 1 : document.getElementById('quantity_value').textContent;
            try {
                const data = await postData('/Cart/Home/AddToCart', {id: id, quantity: quantity});
                if (!data.success) {
                    throw new Error(data.message);
                }
                await AddItemToCart();
                alert(data.message);
            } catch (error) {
                window.location.href = '/Identity/Account/Login';
            }
        })
    })
}
AddToCart();

async function IncreaseQuantity(that){
    const id = that.getAttribute('data-id');
    const quantity = 1;
    try {
        const data = await postData('/Cart/Home/AddToCart', {id: id, quantity: quantity});
        if(data.success){
            const result=  await AddItemToCart();
            document.querySelector('.checkout_items').textContent = result.TotalQuantity;
            document.querySelector('.total_price').textContent = result.TotalPrice;
            document.querySelector('.total_quantity').textContent = result.TotalQuantity;
            let thisElement = document.getElementById(id);
            let price = thisElement.querySelector('.item_price');
            let quantity = thisElement.querySelector('.item_quantity');
            let totalPrice = thisElement.querySelector('.item_total_price');
            quantity.textContent = parseFloat(quantity.textContent) + 1;
            totalPrice.textContent = parseFloat(price.textContent) * parseFloat(quantity.textContent);
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DecreaseQuantity(that){
    const id = that.getAttribute('data-id');
    const quantity = 1;
    try {
        const data = await postData('/Cart/Home/DecreaseToCart', {id: id, quantity: quantity});
        if(data.success){
            const result=  await AddItemToCart();
            document.querySelector('.checkout_items').textContent = result.TotalQuantity;
            document.querySelector('.total_price').textContent = result.TotalPrice;
            document.querySelector('.total_quantity').textContent = result.TotalQuantity;
            let thisElement = document.getElementById(id);
            let price = thisElement.querySelector('.item_price');
            let quantity = thisElement.querySelector('.item_quantity');
            let totalPrice = thisElement.querySelector('.item_total_price');
            quantity.textContent = parseFloat(quantity.textContent) - 1;
            totalPrice.textContent = parseFloat(price.textContent) * parseFloat(quantity.textContent);
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DeleteCartProduct(that){
    const id = that.getAttribute('data-id');
    try {
        const data = await postData('/Cart/Home/DeleteToCart', {id: id, quantity:1});
        if(data.success){
            await AddItemToCart();
            window.location.reload();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}

async function DeleteCartAll(){
    try {
        const data = await postData('/Cart/Home/DeleteToCart', {id: "", quantity:-1});
        if(data.success){
            await AddItemToCart();
            window.location.reload();
        } else {
            alert(data.message);
        }
    } catch (error) {
        console.error(error);
    }
}
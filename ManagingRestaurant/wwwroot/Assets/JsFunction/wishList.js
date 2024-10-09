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

async function addWishList(that, userId, productId,isAdd){
    try {
        const response = await postData('/User/UserNormal/AddWishList', {userId, productId,isAdd});
        if(response.success){
            if(isAdd){
                that.classList.add('active');
            } else {
                that.classList.remove('active');
            }
        } 
        alert(response.message);
    } catch (error) {
        console.error('Fetch error:', error);
    }
}
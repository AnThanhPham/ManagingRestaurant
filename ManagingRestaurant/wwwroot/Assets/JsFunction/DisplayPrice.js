function DisplayPrice(){
    let that = document.querySelectorAll(".d-price");
    that.forEach((item) => {
        let value = parseInt(item.innerText.replace(/./g, '')); // Xóa dấu ',' trước khi chuyển đổi
        // Kiểm tra nếu giá trị là một số hợp lệ
        if (!isNaN(value)) {
            // Biến đổi giá trị thành chuỗi được định dạng
            let formattedValue = value.toLocaleString('en-US');
            // Gán lại giá trị đã định dạng cho input
            item.innerText = formattedValue;
        }
    });
}
DisplayPrice();
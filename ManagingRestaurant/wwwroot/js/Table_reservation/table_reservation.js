function reserveTable(event) {
    event.preventDefault(); // Ngăn chặn hành động mặc định của thẻ <a>

    // Lấy giá trị từ các ô input
    const nameInput = document.querySelector('.reservation-form .form-group:nth-child(1) input');
    const phoneInput = document.querySelector('.reservation-form .form-group:nth-child(2) input');
    const emailInput = document.querySelector('.reservation-form .form-group:nth-child(3) input');
    const guestCountSelect = document.querySelector('.reservation-form .form-group:nth-child(4) select');
    const dateInput = document.querySelector('.reservation-form .form-group:nth-child(5) input');
    const timeInput = document.querySelector('.reservation-form .form-group:nth-child(6) input');

    const reservationData = {
        name: nameInput.value,
        phone: phoneInput.value,
        email: emailInput.value,
        guestCount: guestCountSelect.value,
        date: dateInput.value,
        time: timeInput.value
    };

    // Ở đây bạn có thể thực hiện các xử lý tiếp theo, chẳng hạn như gửi dữ liệu đến server
    console.log(reservationData);
}
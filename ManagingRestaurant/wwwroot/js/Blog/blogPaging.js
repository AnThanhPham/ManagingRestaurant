let currentPage = 1;

function nextPage() { // nut next
    // Ẩn trang hiện tại
    document.getElementById(`page${currentPage}`).style.display = 'none';

    // Tăng số trang hiện tại
    currentPage++;
    if(currentPage > 3) {
        document.querySelector('.page__sma1').style.display = 'none';
        document.querySelector('.page__sma2').style.display = 'none';
        document.querySelector('.page__sma3').style.display = 'none';
        document.querySelector('.page__sma4').style.display = 'block';
        document.querySelector('.page__sma5').style.display = 'block';
        document.querySelector('.page__sma6').style.display = 'block';
    }

    if(currentPage > 6) {
        document.querySelector('.page__sma4').style.display = 'none';
        document.querySelector('.page__sma5').style.display = 'none';
        document.querySelector('.page__sma6').style.display = 'none';
        document.querySelector('.page__sma7').style.display = 'block';
    }
    // Nếu đã hết trang, quay lại trang đầu tiên
    if (currentPage > 7) {
        currentPage = 1;
    }

    if(currentPage == 1) {
        document.querySelector('.page__sma1').style.display = 'block';
        document.querySelector('.page__sma2').style.display = 'block';
        document.querySelector('.page__sma3').style.display = 'block';
        document.querySelector('.page__sma7').style.display = 'none';
    }

    // Hiển thị trang mới
    document.getElementById(`page${currentPage}`).style.display = 'block';
}

function prePage() {  // Nut previous
    // Ẩn trang hiện tại
    document.getElementById(`page${currentPage}`).style.display = 'none';

    // Giam số trang hiện tại
    currentPage--;

    // Nếu trang nho hon 1, quay lại trang cuoi cung
    if (currentPage < 1) {
        currentPage = 7;
    }

    if(currentPage == 7) {
        document.querySelector('.page__sma7').style.display = 'block';
        document.querySelector('.page__sma1').style.display = 'none';
        document.querySelector('.page__sma2').style.display = 'none';
        document.querySelector('.page__sma3').style.display = 'none';
    }

    if(currentPage < 7) {
        document.querySelector('.page__sma7').style.display = 'none';
        document.querySelector('.page__sma4').style.display = 'block';
        document.querySelector('.page__sma5').style.display = 'block';
        document.querySelector('.page__sma6').style.display = 'block';
    }

    if(currentPage < 4) {
        document.querySelector('.page__sma1').style.display = 'block';
        document.querySelector('.page__sma2').style.display = 'block';
        document.querySelector('.page__sma3').style.display = 'block';
        document.querySelector('.page__sma4').style.display = 'none';
        document.querySelector('.page__sma5').style.display = 'none';
        document.querySelector('.page__sma6').style.display = 'none';
    }

    // Hiển thị trang mới
    document.getElementById(`page${currentPage}`).style.display = 'block';
}

function numberPage(number) { // nhấn vào các trang
    // Ẩn tất cả các trang
    document.getElementById(`page${currentPage}`).style.display = 'none';
    // Hiển thị trang được chọn

    currentPage = number;

    var selectedPage = document.getElementById(`page${currentPage}`);
    selectedPage.style.display = 'block';

    // Đặt trang được chọn làm active
    selectedPage.classList.add('active');

    currentPage = number;
}

// Number page pagination

document.addEventListener('DOMContentLoaded', function () {
    const prevPageBtn = document.getElementById('prevPageBtn');
    const prevSvgBtn = document.getElementById('prevSvgBtn');

    prevPageBtn.addEventListener('click', function () {
        changePage('prev');
    });

    prevSvgBtn.addEventListener('click', function () {
        changePage('prev');
    });

    const nextPageBtn = document.getElementById('nextPageBtn');
    const nextSvgBtn = document.getElementById('nextSvgBtn');

    nextPageBtn.addEventListener('click', function () {
        changePage('next');
    });

    nextSvgBtn.addEventListener('click', function () {
        changePage('next');
    });

    function changePage(direction) {
        const activePage = document.querySelector('.page-number-link.active');
        let targetPage;

        if (direction === 'prev') {
            targetPage = activePage.previousElementSibling;
        } else {
            targetPage = activePage.nextElementSibling;
        }

        if (targetPage) {
            activePage.classList.remove('active');
            targetPage.classList.add('active');
        } else {
            const newPage = direction === 'prev' ? document.querySelector('.page-number-link:last-child') : document.querySelector('.page-number-link:first-child');
            activePage.classList.remove('active');
            newPage.classList.add('active');
        }
    }
});

// Small screen page 

document.addEventListener('DOMContentLoaded', function () {
    const prevPageBtn = document.getElementById('prevPageBtn');
    const prevSvgBtn = document.getElementById('prevSvgBtn');

    prevPageBtn.addEventListener('click', function () {
        changePage('prev');
    });

    prevSvgBtn.addEventListener('click', function () {
        changePage('prev');
    });

    const nextPageBtn = document.getElementById('nextPageBtn');
    const nextSvgBtn = document.getElementById('nextSvgBtn');

    nextPageBtn.addEventListener('click', function () {
        changePage('next');
    });

    nextSvgBtn.addEventListener('click', function () {
        changePage('next');
    });

    function changePage(direction) {
        const activePage = document.querySelector('.page-number-link-sm.active');
        let targetPage;

        if (direction === 'prev') {
            targetPage = activePage.previousElementSibling;
        } else {
            targetPage = activePage.nextElementSibling;
        }

        if (targetPage) {
            activePage.classList.remove('active');
            targetPage.classList.add('active');
        } else {
            const newPage = direction === 'prev' ? document.querySelector('.page-number-link-sm:last-child') : document.querySelector('.page-number-link-sm:first-child');
            activePage.classList.remove('active');
            newPage.classList.add('active');
        }
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const pageList = document.getElementById('pageList');
    const pageListSm = document.getElementById('pageListSm');

    const pageLinks = pageList.querySelectorAll('.page-number-link');
    const pageLinksSm = pageListSm.querySelectorAll('.page-number-link-sm');

    pageLinks.forEach(function (pageLink) {
        pageLink.addEventListener('click', function () {
            const pageNumber = pageLink.getAttribute('value');

            // Bỏ lớp 'active' ở số trang hiện tại
            const activePage = document.querySelector('.page-number-link.active');
            activePage.classList.remove('active');

            // Thêm lớp 'active' cho số trang được chọn
            pageLink.classList.add('active');
        });
    });

    // Small screen 
    pageLinksSm.forEach(function (pageLink) {
        pageLink.addEventListener('click', function () {
            const pageNumber = pageLink.getAttribute('value');

            const activePageSm = document.querySelector('.page-number-link-sm.active');
            activePageSm.classList.remove('active');

            pageLink.classList.add('active');
        });
    });
});

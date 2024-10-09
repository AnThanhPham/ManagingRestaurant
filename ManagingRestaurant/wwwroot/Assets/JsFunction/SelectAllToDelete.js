// notiToast function
function notiToast({ title = "", message = "", type = "info", duration = 3000 }) {
    const main = document.getElementById("notiToast");
    if (main) {
        const notiToast = document.createElement("div");

        // Auto remove notiToast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(notiToast);
        }, duration + 1000);

        // Remove notiToast when clicked
        notiToast.onclick = function (e) {
            if (e.target.closest(".notiToast__close")) {
                main.removeChild(notiToast);
                clearTimeout(autoRemoveId);
            }
        };

        const icons = {
            success: "fas fa-check-circle",
            info: "fas fa-info-circle",
            warning: "fas fa-exclamation-circle",
            error: "fas fa-exclamation-circle"
        };
        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        notiToast.classList.add("notiToast", `notiToast--${type}`);
        notiToast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;

        notiToast.innerHTML = `
                    <div class="notiToast__icon">
                        <i class="${icon}"></i>
                    </div>
                    <div class="notiToast__body">
                        <h3 class="notiToast__title">${title}</h3>
                        <p class="notiToast__msg">${message}</p>
                    </div>
                    <div class="notiToast__close">
                        <i class="fas fa-times"></i>
                    </div>
                `;
        main.appendChild(notiToast);
    }
}
function showSuccessnotiToast() {
    notiToast({
        title: "Success!",
        message: "Your request has been successfully processed.",
        type: "success",
        duration: 3000
    });
}

function showErrornotiToast() {
    notiToast({
        title: "Failed!",
        message: "Your request has been failed. Please try again.",
        type: "error",
        duration: 3000
    });
}
async function postData(url = "", data = {}) {
    // Default options are marked with *
    const response = await fetch(url, {
        method: "POST", // *GET, POST, PUT, DELETE, etc.
        headers: {
            "Content-Type": "application/json",
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },

        body: JSON.stringify(data), // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
}

let btnAll = document.getElementById('selectAll');
btnAll.addEventListener('change', function () {
    let changeStatus = btnAll.checked;
    let btns = document.querySelectorAll('.selectItem');
    for (let i = 0; i < btns.length; i++) {
        btns[i].checked = changeStatus;
    }
});

let btnDeleteMany = document.getElementById('btnDeleteMany');
btnDeleteMany.addEventListener('click',  async function () {
    let btnSelected = document.querySelectorAll('.selectItem:checked');
    let strId = "";
    for (let i = 0; i < btnSelected.length; i++) {
        strId += btnSelected[i].value + ",";
    }
    let overlay = document.getElementById("overlay");
    overlay.classList.remove("d-none");
    const url = new URL(window.location.href).pathname;
    console.log(url)
    try {
        
        const response = await postData(`${url}/Home/DeleteMany`, strId);
        if (response.success) {
            showSuccessnotiToast();
            setTimeout(function () {
                location.reload();
            }, 2000);
        } else {
            showErrornotiToast();
            setTimeout(function () {
                location.reload();
            }, 2000);
        }
        overlay.classList.add("d-none");
    } catch (error) {
        console.error(error);
        // Handle error appropriately
    }
});
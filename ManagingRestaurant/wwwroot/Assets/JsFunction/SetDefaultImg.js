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
async function setDefaultImg(that){
    const id = that.getAttribute('data-id');
    const result =  await postData(`/Product/MoreImage/SetDefault`, id)
    if(result.success){
        alert("Set default image success");
        location.reload();
        const createBtnImg = document.querySelector(".create-img");
        createBtnImg.click();
    } else {
        alert("Delete failed" );
        location.reload();
    }
}
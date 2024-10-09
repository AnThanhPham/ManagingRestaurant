document.getElementById('formFile').addEventListener('change', function() {
    var reader = new FileReader();

    reader.onload = function(e) {
        let imgTag = `<img src="${e.target.result}" style="margin: 10px; width: 100px; height: 100px; border: 2px solid #8f8a8a;border-radius: 3px"/>`;
        let imageContainer = document.querySelector('.display-image');
        imageContainer.innerHTML = imgTag;
    };

    reader.readAsDataURL(this.files[0]);
});
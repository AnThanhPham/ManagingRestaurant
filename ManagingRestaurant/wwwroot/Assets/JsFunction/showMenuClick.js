
    let clickList = document.querySelectorAll('.create-click');
    let infoList = document.querySelectorAll('.create-info');

    for(let i=0;i<clickList.length;i++){
    clickList[i].addEventListener('click', function(){
        for(let j=0;j<infoList.length;j++){
            infoList[j].classList.add('d-none');
        }
        for(let j=0;j<clickList.length;j++){
            clickList[j].classList.remove('btn-primary');
        }
        infoList[i].classList.remove('d-none');
        clickList[i].classList.add('btn-primary');
    })
}


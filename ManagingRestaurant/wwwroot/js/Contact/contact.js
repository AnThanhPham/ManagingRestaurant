const namee = document.querySelector(".name");
const emaill = document.querySelector(".email");
const textareaa = document.querySelector("textarea");
const formm = document.querySelector("form");

const showError = (input, message) => {
  input.parentElement.classList.add("error");
  input.parentElement.querySelector("small").innerText = message;
};

const showSuccess = (input) => {
  input.parentElement.classList.remove("error");
  input.parentElement.querySelector("small").innerText = "";
};

const checkEmpty = (listInput) => {
  let isEmptyError = false;

  listInput.forEach((input) => {
    input.value = input.value.trim();

    if (!input.value) {
      isEmptyError = true;
      showError(input, "Cannot be blank");
    } else {
      showSuccess(input);
    }
  });

  return isEmptyError;
};

const checkEmail = (input) => {
  const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
  input.value = input.value.trim();

  let isEmailError = !emailRegex.test(input.value);

  if (emailRegex.test(input.value)) {
    showSuccess(input);
  } else {
    showError(input, "Email Invalid");
  }

  return isEmailError;
};

formm.addEventListener("submit", (e) => {
  e.preventDefault();

  const isEmptyError = checkEmpty([namee, emaill, textareaa]);
  const isEmailError = checkEmail(emaill);

  if (!isEmailError && !isEmptyError) {
    alert(`Your response has been sent`);
    namee.value=``
    emaill.value=``
    textareaa.value=``
  }
});

namee.addEventListener("input", () => showSuccess(namee));
emaill.addEventListener("input", () => showSuccess(emaill));
textareaa.addEventListener("input", () => showSuccess(textareaa));

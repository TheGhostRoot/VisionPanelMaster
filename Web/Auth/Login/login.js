const signInBtnLink = document.querySelector(".signInBtn-link");
const signUpBtnLink = document.querySelector(".signUpBtn-link");
const wrapper = document.querySelector(".wrapper");

signUpBtnLink.addEventListener("click", () => {
  wrapper.classList.toggle("active");
});

signInBtnLink.addEventListener("click", () => {
  wrapper.classList.toggle("active");
});


async function login() {
    const emailObj = document.getElementById("login-email").value;
    const passwordObj = document.getElementById("login-password").value;
    // body: JSON.stringify({ email: emailObj, password: passwordObj })
    // "https://localhost:7125/login?email=" +
    emailObj + "&password=" + passwordObj
    const res = await fetch("https://localhost:7125/login",
        {
            method: "POST",
            JSON: JSON.stringify({ email: emailObj, password: passwordObj })
        });

    console.log(res.status);
}


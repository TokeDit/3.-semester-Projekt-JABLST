/**
 * Selenium tests for Login component
 *
 * These tests verify frontend behavior only (no Firebase/backend).
 *
 * Covered:
 * - Page renders correctly (inputs + buttons visible)
 * - User can input email and password
 * - Validation errors:
 *   - Empty email
 *   - Invalid email format (browser validation)
 *   - Empty password
 *   - Register with password < 6 characters
 * - Error messages update correctly based on user input
 *
 * Not covered:
 * - Firebase authentication (login/register)
 * - Redirect/navigation after success
 */

import { Builder, By, until } from "selenium-webdriver";
import chrome from "selenium-webdriver/chrome.js";
import assert from "assert";

describe("Login component", function () {
  let driver;

  async function getErrorText() {
    const error = await driver.wait(
      until.elementLocated(By.id("error-message")),
      5000,
    );

    return await error.getText();
  }

  beforeEach(async function () {
    this.timeout(60000);

    const options = new chrome.Options();
    options.addArguments("--start-maximized");

    driver = await new Builder()
      .forBrowser("chrome")
      .setChromeOptions(options)
      .build();

    await driver.get("http://localhost:5173/login");

    const loginButton = await driver.wait(
      until.elementLocated(By.id("login-button")),
      10000,
    );

    await driver.wait(until.elementIsVisible(loginButton), 10000);
  });

  afterEach(async function () {
    if (driver) {
      await driver.quit();
    }
  });

  it("shows the login page with required fields and buttons", async function () {
    const emailInput = await driver.findElement(By.id("email-input"));
    const passwordInput = await driver.findElement(By.id("password-input"));
    const loginButton = await driver.findElement(By.id("login-button"));
    const registerButton = await driver.findElement(By.id("register-button"));

    assert.strictEqual(await emailInput.isDisplayed(), true);
    assert.strictEqual(await passwordInput.isDisplayed(), true);
    assert.strictEqual(await loginButton.isDisplayed(), true);
    assert.strictEqual(await registerButton.isDisplayed(), true);

    assert.strictEqual(await loginButton.getText(), "Login");
    assert.strictEqual(await registerButton.getText(), "Create account");
  });

  it("allows user to type email and password", async function () {
    const emailInput = await driver.findElement(By.id("email-input"));
    const passwordInput = await driver.findElement(By.id("password-input"));

    await emailInput.sendKeys("test@test.dk");
    await passwordInput.sendKeys("123456");

    assert.strictEqual(await emailInput.getAttribute("value"), "test@test.dk");
    assert.strictEqual(await passwordInput.getAttribute("value"), "123456");
  });

  it("shows error when email is empty", async function () {
    await driver.findElement(By.id("login-button")).click();

    const text = await getErrorText();

    assert.strictEqual(text, "Email må ikke være tom.");
  });

  it("shows browser validation when email format is invalid", async function () {
    const emailInput = await driver.findElement(By.id("email-input"));

    await emailInput.sendKeys("test");
    await driver.findElement(By.id("password-input")).sendKeys("123456");
    await driver.findElement(By.id("login-button")).click();

    const isValid = await driver.executeScript(
      "return document.getElementById('email-input').checkValidity();",
    );

    const validationMessage =
      await emailInput.getAttribute("validationMessage");

    assert.strictEqual(isValid, false);
    assert.notStrictEqual(validationMessage, "");
  });

  it("shows error when password is empty", async function () {
    await driver.findElement(By.id("email-input")).sendKeys("test@test.dk");
    await driver.findElement(By.id("login-button")).click();

    const text = await getErrorText();

    assert.strictEqual(text, "Password må ikke være tomt.");
  });

  it("updates error message when user fixes email but leaves password empty", async function () {
    await driver.findElement(By.id("login-button")).click();

    let text = await getErrorText();
    assert.strictEqual(text, "Email må ikke være tom.");

    await driver.findElement(By.id("email-input")).sendKeys("test@test.dk");
    await driver.findElement(By.id("login-button")).click();

    text = await getErrorText();
    assert.strictEqual(text, "Password må ikke være tomt.");
  });

  it("shows error when registering with password shorter than 6 characters", async function () {
    await driver.findElement(By.id("email-input")).sendKeys("test@test.dk");
    await driver.findElement(By.id("password-input")).sendKeys("123");
    await driver.findElement(By.id("register-button")).click();

    const text = await getErrorText();

    assert.strictEqual(text, "Password skal være mindst 6 tegn.");
  });
});
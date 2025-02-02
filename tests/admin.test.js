const { Builder, By, Key, until } = require('selenium-webdriver');

describe('MusicMatch User and Admin Tests', () => {
    let driver;

    beforeAll(async () => {
        driver = await new Builder().forBrowser('chrome').build();
        await driver.manage().window().maximize();
    }, 30000);

    afterAll(async () => {
        await driver.quit();
    });

    test('1. Register new user flow', async () => {
        try {
            // Generate unique timestamp for user email
            const userTimestamp = Date.now();
            const testUserEmail = `testuser${userTimestamp}@example.com`;
            
            await driver.get('https://localhost:7064/Identity/Account/Register');
            
            await driver.findElement(By.id('Input_Email')).sendKeys(testUserEmail);
            await driver.findElement(By.id('Input_FirstName')).sendKeys('Test');
            await driver.findElement(By.id('Input_LastName')).sendKeys('User');
            await driver.findElement(By.id('Input_Password')).sendKeys('TestPass123!');
            await driver.findElement(By.id('Input_ConfirmPassword')).sendKeys('TestPass123!');
            await driver.findElement(By.id('registerSubmit')).click();

            await driver.sleep(2000);
            await driver.findElement(By.linkText('Click here to confirm your account')).click();
            await driver.sleep(2000);

            console.log('Successfully registered user:', testUserEmail);
        } catch (error) {
            console.error('User registration test failed:', error);
            throw error;
        }
    }, 20000);

    test('2. Create Genre as Admin', async () => {
        try {
            // Login as admin
            await driver.get('https://localhost:7064/Identity/Account/Login');
            await driver.findElement(By.id('Input_Email')).sendKeys('admin@proiect.com');
            await driver.findElement(By.id('Input_Password')).sendKeys('Admin1!');
            await driver.findElement(By.id('login-submit')).click();
            await driver.sleep(2000);

            // Go to Manage Genres first
            await driver.findElement(By.id('adminDropdown')).click();
            await driver.sleep(1000);
            await driver.findElement(By.linkText('Manage Genres')).click();
            await driver.sleep(1000);
            
            await driver.findElement(By.linkText('Add New Genre')).click();
            await driver.sleep(1000);
            
            const genreTimestamp = Date.now();
            const genreName = `Test Genre ${genreTimestamp}`;
            await driver.findElement(By.id('Name')).sendKeys(genreName);
            await driver.findElement(By.id('Description')).sendKeys('This is a test genre description');
            
            await driver.findElement(By.css('button[type="submit"]')).click();
            await driver.sleep(4000);

            console.log('Successfully created genre:', genreName);
        } catch (error) {
            console.error('Genre creation test failed:', error);
            throw error;
        }
    }, 20000);

    test('3. Create Artist as Admin', async () => {
        try {
            // Login as admin again
            await driver.get('https://localhost:7064/Identity/Account/Login');
            await driver.findElement(By.id('Input_Email')).sendKeys('admin@proiect.com');
            await driver.findElement(By.id('Input_Password')).sendKeys('Admin1!');
            await driver.findElement(By.id('login-submit')).click();
            await driver.sleep(2000);

            // Go to Manage Artists first
            await driver.findElement(By.id('adminDropdown')).click();
            await driver.sleep(1000);
            await driver.findElement(By.linkText('Manage Artists')).click();
            await driver.sleep(1000);
            
            await driver.findElement(By.linkText('Add New Artist')).click();
            await driver.sleep(1000);
            
            const artistTimestamp = Date.now();
            const artistName = `Test Artist ${artistTimestamp}`;
            await driver.findElement(By.id('Name')).sendKeys(artistName);
            await driver.findElement(By.id('Bio')).sendKeys('This is a test artist biography');
            await driver.findElement(By.id('PhotoUrl')).sendKeys('https://example.com/artist.jpg');
            
            await driver.findElement(By.css('button[type="submit"]')).click();
            await driver.sleep(4000);

            console.log('Successfully created artist:', artistName);
        } catch (error) {
            console.error('Artist creation test failed:', error);
            throw error;
        }
    }, 20000);
});
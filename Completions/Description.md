# MusicMatch Selenium E2E Testing

## Overview
Selenium WebDriver tests for MusicMatch application, simulating real user interactions and admin workflows.

On top of the functional testing (our plan can be seen [here](./Completions/Functional-Testing-Plan.pdf)), we performed testing using Selenium. 

## Live Testing
You can see how our application was tested and the results live by watching [this video](https://drive.google.com/drive/folders/1vgYiiV-F5lZbO9EMn_nejsJKz43GUrdB?usp=drive_link).

## Technology Stack
- **Testing Framework**: Jest
- **Automation Tool**: Selenium WebDriver
- **Browser**: Chrome

## Test Scenarios
1. User Registration Flow
2. Admin Genre Creation
3. Admin Artist Creation

## Prerequisites
- Node.js
- Chrome WebDriver
- Running MusicMatch application locally

## Setup

### Install Dependencies
```bash
npm install selenium-webdriver jest
```

### Running Tests
The tests can be ran using
```bash
npm test
```

### Test Details
## 1. User Registration
  - Creates a unique user with timestamp-based email (this was used to prevent errors since when creating the script, it was ran multiple times)
  - Navigates registration flow
  - Confirms account creation

## 2. Genre Creation
  - Logs in as admin
  - Navigates to genre management
  - Creates a new genre with unique name

## 3. Artist Creation
  - Logs in as admin
  - Navigates to artist management
  - Creates a new artist with unique name


## Modified Files
During the implementation of Selenium E2E testing, the following files were added or modified:

### New/Modified Configuration Files
- [`jest.config.js`](./jest.config.js): Jest testing configuration
- [`.env.development`](./env.development): Development environment variables
- [`.env.test`](./env.test): Test environment configuration

### Package Management
- [`package.json`](./package.json): Updated with test scripts and dependencies
- [`package-lock.json`](./package-lock.json): Locked dependency versions

### Testing Artifacts
- [`Tests/`](./Tests): Directory containing Selenium test scripts
- [`coverage/`](./coverage): Test coverage reporting directory


# Databricks

[![Code style: black](https://img.shields.io/badge/code%20style-black-000000.svg)](https://github.com/psf/black)

## Table of content

* [Getting started with Databricks development in Time Series](#getting-started-with-databrick-development-in-time-series)
* [Running Tests](#running-tests)
* [Debugging Tests](#debugging-tests)
* [Styling and Formatting](#styling-and-formatting)

## Getting started with Databricks development in Time Series

### Install necessary tools needed for development

* #### [Docker](https://www.docker.com/get-started)

    * use WSL 2, you will get a prompt with a guide after installing docker

* #### [Visual Studio Code](https://code.visualstudio.com/#alt-downloads) (system installer)

    * Extension called ***Remote - Containers***

### Get workspace ready for development

* Open ***geh-timeseries*** folder in Visual Studio Code

* Select ***Remote Explorer*** in the left toolbar

* Click on the ***plus icon*** in the top of the panel to the right of ***Containers*** and select ***Open Current Folder in Container***

* Wait for the container to build (*This will take a few minutes the first time*) and then you are ready to go

## Running Tests

* To run all test you will need to execute the following command in the workspace terminal

    ```text
    pytest
    ```

* For more verbose output use

    ```text
    pytest -vv -s
    ```

* To run tests in a specific folder simply navigate to the folder in the terminal and use the same command as above

* To run tests in a specific file navigate to the folder where the file is located in the terminal and execute the following command

    ```text
    pytest file-name.py
    ```

* You can also run a specific test in the file by executing the following command

    ```text
    pytest file-name.py::function-name
    ```

## Debugging Tests

* To debug tests you need to execute the following command

    Using debugz.sh with the following command

    ````text
    sh debugz.sh
    ````

    Or using command inside debugz.sh

    ```text
    python -m ptvsd --host 0.0.0.0 --port 3000 --wait -m pytest -v
    ```

* Create a ***launch.json*** file in the ***Run and Debug*** panel and add the following

    ```json
    {
        "name": "Python: Attach container",
        "type": "python",
        "request": "attach",
        "port": 3000,
        "host": "localhost"
    }
    ```

* Start debugging on the ***Python: Attach container*** in the ***Run and Debug*** panel

## Styling and Formatting

We try to follow [PEP8](https://peps.python.org/pep-0008/) as much as possible, we do this by using [Flake8](https://flake8.pycqa.org/en/latest/) and [Black](https://black.readthedocs.io/en/stable/)
The following Flake8 codes are ignored:

* Module imported but unused ([F401](https://www.flake8rules.com/rules/F401.html))
* Module level import not at top of file ([E402](https://www.flake8rules.com/rules/E402.html))
* Whitespace before ':' ([E203](https://www.flake8rules.com/rules/E203.html)) (*Needed for black you work well with Flake8, see documentation [here](https://github.com/psf/black/blob/main/docs/guides/using_black_with_other_tools.md#flake8)*)
* Line too long (82 &gt; 79 characters) ([E501](https://www.flake8rules.com/rules/E501.html)) (*Only ignored in CI step*)

We are using standard [Black code style](https://github.com/psf/black/blob/main/docs/the_black_code_style/current_style.md#the-black-code-style).

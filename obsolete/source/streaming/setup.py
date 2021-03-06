# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
from setuptools import setup, find_packages
from setuptools.command.develop import develop

import pathlib
import os
from subprocess import check_call


def generate_proto_code():
    proto_path = "../contracts/internal/"
    python_out = "geh_stream/contracts/"

    os.makedirs(python_out, exist_ok=True)
    check_call(["protoc"] + ["time_series_command.proto"] + ["--python_out", python_out, "--proto_path", proto_path])


class CustomDevelopCommand(develop):
    """Wrapper for custom commands to run before package installation."""
    uninstall = False

    def run(self):
        develop.run(self)

    def install_for_development(self):
        generate_proto_code()
        develop.install_for_development(self)


# File 'VERSION' is created by pipeline. If executed manual it must be created manually.
__version__ = ""
with open('VERSION') as version_file:
    __version__ = version_file.read().strip()

setup(name='geh_stream',
      version=__version__,
      description='Tools for streaming and aggregation of meter data of Green Energy Hub',
      long_description="",
      long_description_content_type='text/markdown',
      license='MIT',
      packages=find_packages(),
      cmdclass={
           'develop': CustomDevelopCommand,  # used for pip install -e ./
      },
      )

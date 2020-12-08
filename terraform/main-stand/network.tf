variable "base_subnet" {
  default = "10.118.0.0/16"
}

variable "subnet_id" {
  default = "e2lrmm47psoq42amkfve"
}

locals {
  jury_subnet = cidrsubnet(var.base_subnet, 8, 0)
}

### Notes
# 10.118.0.0/24 - jury
# 10.118.0.5 - infra stuff
# 10.118.0.10-11 - checksystem
# 10.118.0.20-21 - dns + nginx
# 10.118.101-110.0/24 - teams
# 10.118.1xx.10 - router
# 10.118.1xx.11 - s1
# 10.118.1xx.12 - s2
# 10.118.1xx.13 - s3

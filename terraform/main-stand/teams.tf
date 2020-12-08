### teams stuff

module "teams" {
  source = "../modules/team"

  count = 10

  subnet_id = var.subnet_id
  team_subnet = cidrsubnet(var.base_subnet, 8, 101 + count.index)
  jury_subnet = local.jury_subnet
  base_image_id = local.ubuntu-with-docker-id
  instance_prefix = "team${101 + count.index}"

  ssh_keys = [local.jury_ssh_key, file("${path.root}/../../teams/${101 + count.index}/ssh_key.pub")]
  serial_ssh_key = file("${path.root}/../../teams/${101 + count.index}/ssh_key.pub")
}

resource "local_file" "team_fip" {
  count = 10
  content = module.teams[count.index].main_fip
  filename = "${path.root}/../../teams/${101 + count.index}/main_fip"
}

output "team_ids" {
  value = module.teams[*].main_id
}

output "team_fip" {
  value = module.teams[*].main_fip
}

